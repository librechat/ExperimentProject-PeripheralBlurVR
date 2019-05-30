library(ggplot2)
library(gcookbook)
library(rlist)

#file folder
folderName = "./Pilot1"
conditionNames = c("StaticSmall","StaticLarge","Baseline")
sessionNames = c("Session1","Session2","Session3")

#library(wesanderson)
#color = wes_palette(n=length(folderNames), name="Darjeeling1")
library(RColorBrewer)
color = brewer.pal(n=length(conditionNames), name = "Set2")

defaultpar = par()
par(mar=c(12,6,1,1))
fontsize=1

temp = list.files(path=folderName ,pattern="*.csv")

condition = lapply(temp, function(t){
  slist = unlist(strsplit(t, "_", fixed=TRUE))
  if(slist[2] == conditionNames[1]) return(1)
  else if(slist[2]==conditionNames[2]) return(2)
  else if(slist[2]==conditionNames[3]) return(3)
  else return(0)
})
condition = unlist(condition)
participant = lapply(temp, function(t){
  slist = unlist(strsplit(t, "_", fixed=TRUE))
  par = as.numeric(substr(slist[1],start=12,stop=12))
  return (par)
})
participant = unlist(participant)
session = lapply(temp, function(t){
  slist = unlist(strsplit(t, "_", fixed=TRUE))
  ses = as.numeric(substr(slist[3],start=8,stop=8))
  return (ses)
})
session = unlist(session)

discomfortRecord = lapply(temp, function(t){
  default = data.frame(
    Index = 0,
    TaskIndex = -1,
    StartTimeStamp = 0,
    QustionedTimeStamp = 0,
    EndTimeStamp = 0,
    Discomfort = 1
  )
  
  filename = paste(folderName, "/", t, sep="")
  tb = read.table(file=filename, header=TRUE, sep = ",")
  tb$Index = tb$Index+1
  return(rbind(default, tb))
})

Draw = function(conditionIndex=0, participantIndex=0, sessionIndex = 0,
                printAverage=TRUE, groupWithSession=FALSE,gradientByAnotherFactor=FALSE){
  plot(NULL, xlim=c(0,20), ylim=c(1,10), type="l", xlab="", ylab="",
       cex.lab=fontsize, cex.axis=fontsize, cex.main=fontsize, cex.sub=fontsize, xaxt="n", yaxt="n")
  
  title(xlab="Time (minute)", mgp=c(2, 2, 0), cex.lab=fontsize)
  axis(1, at=seq(0,20,1), cex.axis=fontsize)
  
  title(ylab="Discomfort Score", mgp=c(2, 1, 0), cex.lab=fontsize)
  axis(2, at=seq(1,10,1), cex.axis=fontsize)
  
  #draw lines
  lapply(seq_along(discomfortRecord), function(index){
    if(conditionIndex != 0 && conditionIndex != condition[index]) return (NULL)
    if(participantIndex != 0 && participantIndex != participant[index]) return(NULL)
    if(sessionIndex != 0 && sessionIndex != session[index]) return(NULL)
    
    c = color[condition[index]]
    if(groupWithSession) c = color[session[index]]
    width = 1
    if(gradientByAnotherFactor){
      if(groupWithSession) width = condition[index]
      else width = session[index]
    }
    
    lines(discomfortRecord[[index]]$Index, discomfortRecord[[index]]$Discomfort,
          col=c,lwd=width)
  })
  
  #compute average line
  if(participantIndex == 0 && printAverage){
    group = seq_along(conditionNames)
    if(groupWithSession) group = seq_along(sessionNames)
    
    lapply(group, function(groupIndex){
      timeStamp = c(0:20)
      timeStampIndex = seq.int(length(timeStamp))
      
      averageDiscomfort = lapply(timeStampIndex, function(t){
        sum = 0
        valueAtTimeStamp = sapply(seq_along(discomfortRecord), function(index){
          if(groupWithSession && session[index] != groupIndex) return(0)
          if(!groupWithSession && condition[index] != groupIndex) return(0)
          
          if(t <= length(discomfortRecord[[index]]$Discomfort)) return (discomfortRecord[[index]]$Discomfort[t])
          else return (discomfortRecord[[index]]$Discomfort[length(discomfortRecord[[index]]$Discomfort)])
        })
        avg = mean(valueAtTimeStamp[valueAtTimeStamp!=0])
        return(avg)
      })
      #draw average line
      lines(timeStamp, averageDiscomfort, col=color[groupIndex], lwd=4)
    })
  }
  
  #print legends
  groupNames = conditionNames
  anotherNames = sessionNames
  if(groupWithSession) {
    groupNames = sessionNames
    anotherNames = conditionNames
  }
  if(printAverage){
    if(gradientByAnotherFactor){
      legend(x=0, y=-3,
             legend = c(paste(rep(groupNames,times=2),rep(c("","(Average)"),each=3),sep=" "),anotherNames),
             col = c(rep(color, times=2),rep("#000000",times=3)),
             pch = rep(NA, times=9),
             lwd = c(rep(c(1,4),each=3),1,2,3),
             cex=fontsize*0.8, y.intersp = 1, ncol=3, bty="n", xpd=TRUE)
    } else {
      legend(x=0,y=-3,
             legend=paste(rep(groupNames,times=2),rep(c("","(Average)"),each=3),sep=" "),
             col=rep(color, times=2), pch = rep(NA, times=6), lwd = rep(c(1,4),each=3),
             cex=fontsize*0.8, y.intersp = 1, ncol=2, bty="n", xpd=TRUE)
    }
  }
  else {
    if(gradientByAnotherFactor){
      legend(x=0,y=-3,
             legend=c(groupNames,anotherNames),
             col=c(color,rep("#000000",times=3)), pch = rep(NA, times=6), lwd = c(rep(1,times=3),1,2,3),
             cex=fontsize*0.8, y.intersp = 1, ncol=2, bty="n", xpd=TRUE)
    } else{
      legend(x=0,y=-3,
             legend=groupNames,
             col=color, pch = rep(NA, times=3), lwd = 1,
             cex=fontsize*0.8, y.intersp = 1, ncol=1, bty="n", xpd=TRUE)
    }
  }
}
Draw()