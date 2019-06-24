library(ggplot2)
library(gcookbook)
library(rlist)
library(RColorBrewer)

defaultpar = par()
par(mar=c(12,6,1,1))
fontsize=1

folderName = "./Study1_Pilot2/Spatial/"
temp = list.files(path=folderName ,pattern="*.csv")

conditionNames = c("StaticSmall","StaticLarge","Baseline")
sessionNames = c("Session1","Session2","Session3")

color = brewer.pal(n=length(conditionNames), name = "Set2")

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

spatialRecord = lapply(temp, function(t){
  filename = paste(folderName, "/", t, sep="")
  tb = read.table(file=filename, header=TRUE, sep = ",")
  
  return(tb)
})

avgError = lapply(spatialRecord, function(tb){
  return(mean(tb$ErrorOnPlane))
})

CompareSpatial = function(groupWithSession = FALSE){
  group = seq_along(conditionNames)
  if(groupWithSession) group = seq_along(sessionNames)
  
  group_mean = sapply(group, function(groupIndex){
    values = sapply(seq_along(avgError), function(avgIndex){
      if(groupWithSession && session[avgIndex] != groupIndex) return(0)
      if(!groupWithSession && condition[avgIndex] != groupIndex) return(0)
      
      return(avgError[[avgIndex]])
    })
    return(mean(values[values!=0]))
  })
  
  if(groupWithSession) print(sessionNames)
  else print(conditionNames)
  print(group_mean)
}
DrawSpatialError = function(conditionIndex=0, participantIndex=0, sessionIndex = 0,
                            printAverage=TRUE, groupWithSession=FALSE,gradientByAnotherFactor=FALSE){
  plot(NULL, xlim=c(0,20), ylim=c(0,140), type="l", xlab="", ylab="",
       cex.lab=fontsize, cex.axis=fontsize, cex.main=fontsize, cex.sub=fontsize, xaxt="n", yaxt="n")
  
  title(xlab="Time (minute)", mgp=c(2, 2, 0), cex.lab=fontsize)
  axis(1, at=seq(0,20,1), cex.axis=fontsize)
  
  title(ylab="Error (degree)", mgp=c(2, 1, 0), cex.lab=fontsize)
  axis(2, at=seq(0,140,20), cex.axis=fontsize)
  
  #draw lines
  lapply(seq_along(spatialRecord), function(index){
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
    
    timeStampMinute = spatialRecord[[index]]$EndTimeStamp / 60;
    lines(timeStampMinute, spatialRecord[[index]]$ErrorOnPlane,
          col=c,lwd=width)
  })
  
  #print legends
  groupNames = conditionNames
  anotherNames = sessionNames
  if(groupWithSession) {
    groupNames = sessionNames
    anotherNames = conditionNames
  }
  if(printAverage){
    if(gradientByAnotherFactor){
      legend(x=0, y=-50,
             legend = c(paste(rep(groupNames,times=2),rep(c("","(Average)"),each=3),sep=" "),anotherNames),
             col = c(rep(color, times=2),rep("#000000",times=3)),
             pch = rep(NA, times=9),
             lwd = c(rep(c(1,4),each=3),1,2,3),
             cex=fontsize*0.8, y.intersp = 1, ncol=3, bty="n", xpd=TRUE)
    } else {
      legend(x=0,y=-50,
             legend=paste(rep(groupNames,times=2),rep(c("","(Average)"),each=3),sep=" "),
             col=rep(color, times=2), pch = rep(NA, times=6), lwd = rep(c(1,4),each=3),
             cex=fontsize*0.8, y.intersp = 1, ncol=2, bty="n", xpd=TRUE)
    }
  }
  else {
    if(gradientByAnotherFactor){
      legend(x=0,y=-50,
             legend=c(groupNames,anotherNames),
             col=c(color,rep("#000000",times=3)), pch = rep(NA, times=6), lwd = c(rep(1,times=3),1,2,3),
             cex=fontsize*0.8, y.intersp = 1, ncol=2, bty="n", xpd=TRUE)
    } else{
      legend(x=0,y=-50,
             legend=groupNames,
             col=color, pch = rep(NA, times=3), lwd = 1,
             cex=fontsize*0.8, y.intersp = 1, ncol=1, bty="n", xpd=TRUE)
    }
  }
}

CompareSpatial()
DrawSpatialError()