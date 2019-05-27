library(ggplot2)
library(gcookbook)
library(rlist)

#print lines options
options = data.frame(
  participantIndex = 0, # 0 for print all
  conditionIndex = 0,
  printAverage = TRUE
)

#file folder
conditionNames = c("Small","Large","Baseline")
folderNames = c("./Pilot1/StaticSmall", "./Pilot1/StaticLarge", "./Pilot1/Baseline")

#library(wesanderson)
#color = wes_palette(n=length(folderNames), name="Darjeeling1")
library(RColorBrewer)
color = brewer.pal(n=length(folderNames), name = "Set2")

defaultpar = par()
par(mar=c(12,6,1,1))
fontsize=1
plot(NULL, xlim=c(0,20), ylim=c(1,10), type="l", xlab="", ylab="",
     cex.lab=fontsize, cex.axis=fontsize, cex.main=fontsize, cex.sub=fontsize, xaxt="n", yaxt="n")
title(xlab="Time (minute)", mgp=c(4.5, 2, 0), cex.lab=fontsize)
title(ylab="Discomfort Score", mgp=c(3.5, 1, 0), cex.lab=fontsize)
axis(1, at=seq(0,20,1), cex.axis=fontsize, mgp=c(3, 2, 0))
axis(2, at=seq(1,10,1), cex.axis=fontsize)

lapply(seq_along(folderNames), function(index){
  temp = list.files(path=folderNames[index] ,pattern="*.csv")
  
  discomfortRecord = lapply(temp, function(t){
    filename = paste(folderNames[index], "/", t, sep="")
    read.table(file=filename, header=TRUE, sep = ",")
  })
  
  conditionIndex = options[["conditionIndex"]]
  if(conditionIndex != 0 && conditionIndex != index) return()
  
  discomfortRecord = lapply(discomfortRecord, function(discomfort){
    discomfort$Index = discomfort$Index+1
    return(discomfort)
  })
  
  default = data.frame(
    Index = 0,
    TaskIndex = -1,
    StartTimeStamp = 0,
    QustionedTimeStamp = 0,
    EndTimeStamp = 0,
    Discomfort = 1
  )
  discomfortRecord = lapply(discomfortRecord, function(discomfort){
    return (rbind(default, discomfort))
  })
  
  participantIndex = options[["participantIndex"]]
  if(participantIndex == 0){
    #draw lines for all participants
    lapply(discomfortRecord, function(discomfort){
      lines(discomfort$Index, discomfort$Discomfort, col=color[index])
    })
  }
  else {
    #draw lines for single participant
    lines(discomfortRecord[[participantIndex]]$Index, discomfortRecord[[participantIndex]]$Discomfort, col=color[index])
  }
  
  #compute average line
  timeStamp = c(0:20)
  averageDiscomfort = seq.int(length(timeStamp))
  timeStampIndex = seq.int(length(timeStamp))
  for(t in timeStampIndex){
    sum = 0
    valueAtTimeStamp = sapply(discomfortRecord, function(discomfort){
      if(t <= length(discomfort$Discomfort)) return (discomfort$Discomfort[t])
      else return (discomfort$Discomfort[length(discomfort$Discomfort)])
    })
    avg = mean(valueAtTimeStamp)
    averageDiscomfort[t] = avg
  }
  #draw average line
  if(participantIndex == 0 && options[["printAverage"]]) lines(timeStamp, averageDiscomfort, col=color[index], lwd=4)
})
legend(x=0,y=-5,
       legend=paste(rep(conditionNames,times=2),rep(c("","(Average)"),each=3),sep=" "),
       col=rep(color, times=2), pch = rep(NA, times=6), lwd = rep(c(1,4),each=3),
       cex=fontsize*0.8, y.intersp = 1, ncol=2, bty="n", xpd=TRUE)