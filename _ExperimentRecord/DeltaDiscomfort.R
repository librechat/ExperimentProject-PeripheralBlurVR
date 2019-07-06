library(ggplot2)
library(gcookbook)
library(rlist)

#file folder
folderName <- "./Study1/Records/Discomfort"
conditionNames <- c("Baseline","StaticLarge","StaticSmall")
sessionNames <- c("Session1","Session2","Session3")

#library(wesanderson)
#color <- wes_palette(n=length(folderNames), name=Darjeeling1")
library(RColorBrewer)
color <- brewer.pal(n=length(conditionNames), name="Set2")

defaultpar <- par()
par(mar<-c(12,6,1,1))
fontsize<-1

temp <- list.files(path=folderName ,pattern="*.csv")

condition <- lapply(temp, function(t){
  slist <- unlist(strsplit(t, "_", fixed<-TRUE))
  if(slist[2] == conditionNames[1]) return(1)
  else if(slist[2]==conditionNames[2]) return(2)
  else if(slist[2]==conditionNames[3]) return(3)
  else return(0)
})
condition <- unlist(condition)
conditionString <- lapply(temp, function(t){
  slist <- unlist(strsplit(t, "_", fixed<-TRUE))
  return (slist[2])
})
conditionString <- unlist(conditionString)
participant <- lapply(temp, function(t){
  slist <- unlist(strsplit(t, "_", fixed<-TRUE))
  par <- as.numeric(substr(slist[1],start<-12,stop<-100))
  return (par)
})
participant <- unlist(participant)
session <- lapply(temp, function(t){
  slist <- unlist(strsplit(t, "_", fixed<-TRUE))
  ses <- as.numeric(substr(slist[3],start<-8,stop<-8))
  return (ses)
})
session <- unlist(session)

discomfortRecord <- lapply(seq_along(temp), function(tempIndex){
  default <- data.frame(
    Index=0,
    TaskIndex=1,
    StartTimeStamp=0,
    QustionedTimeStamp=0,
    EndTimeStamp=0,
    Discomfort=1
  )
  
  filename <- paste(folderName, "/", temp[tempIndex], sep="")
  tb <- read.table(file=filename, header=TRUE, sep=",")
  tb$Index <- tb$Index+1
  tb <- rbind(default, tb)
  tb$Participant <- participant[tempIndex]
  tb$Session <- session[tempIndex]
  tb$Condition <- conditionString[tempIndex]
  return(tb)
})

extrudedDiscomfortRecord <- lapply(discomfortRecord, function(record){
  lastIndex <- record$Index[length(record$Index)]
  lastDiscomfort <- record$Discomfort[length(record$Discomfort)]
  
  start <- lastIndex+1
  length <- 10-lastIndex
  
  if(lastIndex < 10){
    extrude <- data.frame(
      Index=c(start:10),
      TaskIndex=rep(-1, length),
      StartTimeStamp=rep(0, length),
      QustionedTimeStamp=rep(0, length),
      EndTimeStamp=c(start : 10)*2,
      Discomfort=rep(lastDiscomfort, length),
      Participant=rep(record$Participant[1], length),
      Session=rep(record$Session[1], length),
      Condition=rep(record$Condition[1],length)
    )
    return (rbind(record, extrude))
  }
  else return(record)
})
deltaDiscomfortRecord <- extrudedDiscomfortRecord

participantLevel <- as.numeric(levels(as.factor(participant)))
baselineDataIndex <- participantLevel
session1DataIndex <- participantLevel
for(index in seq_along(participant)){
  p <- match(participant[index], participantLevel)
  if(session[index] == 1){
    session1DataIndex[p] <- index
  }
  if(condition[index] == 1){
    baselineDataIndex[p] <- index
  }
}
deltaConditionDiscomfortRecord <- lapply(seq_along(extrudedDiscomfortRecord), function(index){
  p <- match(participant[index], participantLevel)
  base <- baselineDataIndex[p]
  record <- extrudedDiscomfortRecord[[index]]
  record$Discomfort <- record$Discomfort - extrudedDiscomfortRecord[[base]]$Discomfort
  return(record)
})
deltaSessionDiscomfortRecord <- lapply(seq_along(extrudedDiscomfortRecord), function(index){
  p <- match(participant[index], participantLevel)
  base <- session1DataIndex[p]
  record <- extrudedDiscomfortRecord[[index]]
  record$Discomfort <- record$Discomfort - extrudedDiscomfortRecord[[base]]$Discomfort
  return(record)
})

Draw <- function(conditionIndex=0, participantIndex=0, sessionIndex = 0,
                printLines=FALSE, printAverage=TRUE, groupWithSession=FALSE,gradientByAnotherFactor=FALSE){
  # compute delta values
  group <- seq_along(conditionNames)
  if(groupWithSession) group <- seq_along(sessionNames)
  
  plot(NULL, xlim=c(0,20), ylim=c(-5,3), type="l", xlab="", ylab="",
       cex.lab=fontsize, cex.axis=fontsize, cex.main=fontsize, cex.sub=fontsize, xaxt="n", yaxt="n")
  
  title(xlab="Time (minute)", mgp=c(2, 2, 0), cex.lab=fontsize)
  axis(1, at=seq(0,20,1), cex.axis=fontsize)
  
  title(ylab="Delta Discomfort Score", mgp=c(2, 1, 0), cex.lab=fontsize)
  axis(2, at=seq(-5,5,1), cex.axis=fontsize)
  
  lines(c(-2,22), c(0,0),
        col="#000000",lwd=1)
  
  #draw lines
  if(printLines) DrawLines(conditionIndex, participantIndex, sessionIndex,
                           printAverage, groupWithSession, gradientByAnotherFactor)
  
  if(printAverage) DrawAverage(groupWithSession, gradientByAnotherFactor)
  
  #print legends
  groupNames <- conditionNames
  anotherNames <- sessionNames
  if(groupWithSession) {
    groupNames <- sessionNames
    anotherNames <- conditionNames
  }
  if(printAverage){
    if(gradientByAnotherFactor){
      legend(x=0, y=5,
             legend=c(paste(rep(groupNames,times<-2),rep(c("","(Average)"),each=3),sep=" "),anotherNames),
             col=c(rep(color, times=2),rep("#000000",times=3)),
             pch=rep(NA, times=9),
             lwd=c(rep(c(1,4),each=3),1,2,3),
             cex=fontsize*0.8, y.intersp=1, ncol=3, bty="n", xpd=TRUE)
    } else {
      legend(x=0,y=5,
             legend=paste(rep(groupNames,times=2),rep(c("","(Average)"),each=3),sep=" "),
             col=rep(color, times=2), pch=rep(NA, times=6), lwd=rep(c(1,4),each=3),
             cex=fontsize*0.8, y.intersp=1, ncol=2, bty="n", xpd=TRUE)
    }
  }
  else {
    if(gradientByAnotherFactor){
      legend(x=0,y=5,
             legend=c(groupNames,anotherNames),
             col=c(color,rep("#000000",times=3)), pch=rep(NA, times<-6), lwd=c(rep(1,times<-3),1,2,3),
             cex=fontsize*0.8, y.intersp=1, ncol=2, bty="n", xpd=TRUE)
    } else{
      legend(x=0,y=5,
             legend=groupNames,
             col=color, pch=rep(NA, times=3), lwd=1,
             cex=fontsize*0.8, y.intersp=1, ncol=1, bty="n", xpd=TRUE)
    }
  }
}
DrawLines <- function(conditionIndex=0, participantIndex=0, sessionIndex=0,
                     printAverage=TRUE, groupWithSession=FALSE,gradientByAnotherFactor=FALSE){
  lapply(seq_along(discomfortRecord), function(index){
    if(conditionIndex != 0 && conditionIndex != condition[index]) return (NULL)
    if(participantIndex != 0 && participantIndex != participant[index]) return(NULL)
    if(sessionIndex != 0 && sessionIndex != session[index]) return(NULL)
    
    c <- color[condition[index]]
    if(groupWithSession) c <- color[session[index]]
    width <- 1
    if(gradientByAnotherFactor){
      if(groupWithSession) width <- condition[index]
      else width <- session[index]
    }
    
    if(groupWithSession){
      lines(deltaSessionDiscomfortRecord[[index]]$EndTimeStamp, deltaSessionDiscomfortRecord[[index]]$Discomfort,
            col=c,lwd=width)
    }
    else{
      lines(deltaConditionDiscomfortRecord[[index]]$EndTimeStamp, deltaConditionDiscomfortRecord[[index]]$Discomfort,
            col=c,lwd=width)
    }
  })
}
DrawAverage <- function(groupWithSession=FALSE,gradientByAnotherFactor=FALSE,printArrows=TRUE){
  
  #plot(NULL, xlim<-c(0,20), ylim<-c(1,10), type<-"l", xlab<-"", ylab<-"",
  #cex.lab<-fontsize, cex.axis<-fontsize, cex.main<-fontsize, cex.sub<-fontsize, xaxt<-"n", yaxt<-"n")
  
  #compute average line
  group <- seq_along(conditionNames)
  if(groupWithSession) group <- seq_along(sessionNames)
  
  deltaDiscomfortRecord <- deltaConditionDiscomfortRecord
  if(groupWithSession) deltaDiscomfortRecord <- deltaSessionDiscomfortRecord
  
  lapply(group, function(groupIndex){
    if(groupIndex == 1) return()
    timeStamp <- seq(0,20,2)
    timeStampIndex <- seq.int(length(timeStamp))
    
    discomfortSumup <- lapply(timeStampIndex, function(t){
      sum <- 0
      valueAtTimeStamp <- sapply(seq_along(deltaDiscomfortRecord), function(index){
        if(groupWithSession && session[index] != groupIndex) return(1000)
        if(!groupWithSession && condition[index] != groupIndex) return(1000)
        
        return (deltaDiscomfortRecord[[index]]$Discomfort[t])
      })
      
      values <- valueAtTimeStamp[valueAtTimeStamp<1000]
      avg <- mean(valueAtTimeStamp[valueAtTimeStamp<1000])
      sd <- sd(valueAtTimeStamp[valueAtTimeStamp<1000])
      
      #if(t>1){
      #  print(shapiro.test(values))
      #}
      
      d <- data.frame(
        Avg <- avg,
        Sd <- sd
      )
      return(d)
    })
    discomfortSumup <- do.call(rbind,discomfortSumup)
    #draw average line
    lines(timeStamp, discomfortSumup$Avg, col=color[groupIndex], lwd=4)
    if(printArrows){
      arrows(timeStamp,discomfortSumup$Avg, timeStamp,discomfortSumup$Avg+discomfortSumup$Sd, length=0.05, angle=90)
      arrows(timeStamp,discomfortSumup$Avg, timeStamp,discomfortSumup$Avg-discomfortSumup$Sd, length=0.05, angle=90)
    }
  })
}

Draw()

Compare = function(groupWithSession = FALSE){
  
  deltaDiscomfortRecord <- deltaConditionDiscomfortRecord
  if(groupWithSession) deltaDiscomfortRecord <- deltaSessionDiscomfortRecord
  
  # linear mixed model
  # Regroup discomfortRecord
  # H0: mA<-mB<-mC
  # Problem of this method: different number of samples with different participant
  discomfortTotal <- do.call(rbind,deltaDiscomfortRecord)
  discomfortTotal$Participant <- as.factor(discomfortTotal$Participant)
  discomfortTotal$Session <- as.factor(discomfortTotal$Session)
  library(nlme)
  lmeCondition1 <- summary(lme(Discomfort~Condition+EndTimeStamp,random=~1|Participant,data=discomfortTotal,method="ML"))
  lmeCondition2 <- summary(lme(Discomfort~Condition+EndTimeStamp+Condition*EndTimeStamp,random=~1|Participant,data=discomfortTotal,method="ML"))
  lmeSession1 <- summary(lme(Discomfort~Session+EndTimeStamp,random=~1|Participant,data=discomfortTotal,method="ML"))
  lmeSession2 <- summary(lme(Discomfort~Session+EndTimeStamp+Session*EndTimeStamp,random=~1|Participant,data=discomfortTotal,method="ML"))
  lmeTotal <- summary(lme(Discomfort~Condition+Session+EndTimeStamp+Condition*EndTimeStamp+Session*EndTimeStamp+Condition*Session+Condition*Session*EndTimeStamp,random=~1|Participant,data=discomfortTotal,method="ML"))
  
  print(lmeCondition2)
}

Shapiro = function(groupWithSession=FALSE){
  discomfortTotal <- do.call(rbind,deltaDiscomfortRecord)
  print(shapiro.test(discomfort.Total))
}