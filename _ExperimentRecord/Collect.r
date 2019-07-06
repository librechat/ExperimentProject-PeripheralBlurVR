library(ggplot2)
library(gcookbook)
library(rlist)
library(RColorBrewer)

#defaultpar = par()
#par(mar=c(12,6,1,1))
fontsize=1

folderName = "./Study1/Records/Collect/"
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
conditionString = lapply(temp, function(t){
  slist = unlist(strsplit(t, "_", fixed=TRUE))
  return(slist[2])
})
conditionString = unlist(conditionString)
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

collectRecord = lapply(temp, function(t){
  filename = paste(folderName, "/", t, sep="")
  tb = read.table(file=filename, header=TRUE, sep = ",")
  
  tb$ExecuteTime = tb$EndTimeStamp - tb$StartTimeStamp
  
  return(tb)
})

avgExecuteTime = lapply(collectRecord, function(tb){
  return(mean(tb$ExecuteTime))
})

CompareCollect = function(groupWithSession = FALSE){
  group = seq_along(conditionNames)
  if(groupWithSession) group = seq_along(sessionNames)
  
  group_mean = sapply(group, function(groupIndex){
    values = sapply(seq_along(avgExecuteTime), function(avgIndex){
      if(groupWithSession && session[avgIndex] != groupIndex) return(0)
      if(!groupWithSession && condition[avgIndex] != groupIndex) return(0)
      
      return(avgExecuteTime[[avgIndex]])
    })
    return(mean(values[values!=0]))
  })
  
  if(groupWithSession) print(sessionNames)
  else print(conditionNames)
  print(group_mean)
}
DrawCollect = function(conditionIndex=0, participantIndex=0, sessionIndex = 0,
                            printAverage=TRUE, groupWithSession=FALSE,gradientByAnotherFactor=FALSE){
  plot(NULL, xlim=c(0,20), ylim=c(0,100), type="l", xlab="", ylab="",
       cex.lab=fontsize, cex.axis=fontsize, cex.main=fontsize, cex.sub=fontsize, xaxt="n", yaxt="n")
  
  title(xlab="Time (minute)", mgp=c(2, 2, 0), cex.lab=fontsize)
  axis(1, at=seq(0,20,1), cex.axis=fontsize)
  
  title(ylab="Execute Time (second)", mgp=c(2, 1, 0), cex.lab=fontsize)
  axis(2, at=seq(0,100,20), cex.axis=fontsize)
  
  #draw lines
  lapply(seq_along(collectRecord), function(index){
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
    
    timeStampMinute = collectRecord[[index]]$EndTimeStamp / 60;
    lines(timeStampMinute, collectRecord[[index]]$ExecuteTime,
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

CompareCollect()
#DrawCollect()

AvgCollect = data.frame(Participant=participant,Condition=conditionString,Session=session,ExecuteTime=unlist(avgExecuteTime))
AvgCollect$Participant = as.factor(AvgCollect$Participant)
AvgCollect$Session = as.factor(AvgCollect$Session)
plot(ExecuteTime~Condition,data=AvgCollect)
plot(ExecuteTime~Session, data=AvgCollect)

# 1 way ANOVA
print(summary(aov(ExecuteTime~Condition+Error(Participant),data=AvgCollect)))
print(summary(aov(ExecuteTime~Session+Error(Participant),data=AvgCollect)))
# 2 way ANOVA
print(summary(aov(ExecuteTime~Condition*Session+Error(Participant),data=AvgCollect)))
#lme
print(summary(lme(ExecuteTime~Condition,random=~1|Participant,data=AvgCollect,method="ML")))
print(summary(lme(ExecuteTime~Session,random=~1|Participant,data=AvgCollect,method="ML")))