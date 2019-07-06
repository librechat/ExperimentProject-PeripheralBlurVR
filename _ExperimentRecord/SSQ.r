folderName = "./Study1/SSQ/"

ComputeSSQ = function(SSQ){
  score_N =
    (SSQ$General.discomfort + SSQ$Salivation.increasing + SSQ$Sweating + SSQ$Nausea +
       SSQ$Difficulty.concentrating + SSQ$Stomach.awareness + SSQ$Burping)
  score_O = 
    (SSQ$General.discomfort + SSQ$Fatigue + SSQ$Headache + SSQ$Eye.strain + SSQ$Difficulty.focusing +
       SSQ$Difficulty.concentrating + SSQ$Blurred.vision)
  score_D =
    (SSQ$Difficulty.focusing + SSQ$Nausea + SSQ$Fullness.of.the.Head + SSQ$Blurred.vision +
       SSQ$Dizziness.with.eyes.open + SSQ$Dizziness.with.eyes.closed + SSQ$Vertigo)
  SSQ$score_TS = score_N + score_O + score_D * 3.74
  SSQ$score_N = score_N * 9.54
  SSQ$score_O = score_O * 7.58
  SSQ$score_D = score_D * 13.92
  
  #print(tapply(SSQ$score_TS, SSQ$Condition, mean))
  #print(tapply(SSQ$score_TS, SSQ$Session, mean))
  
  return(SSQ)
}

CompareWithCondition = function(pre, post){
  SSQ1 = tapply(pre$score_TS, pre$Condition, function(ts){
    return(ts)
  })
  SSQ2 = tapply(post$score_TS, post$Condition, function(ts){
    return(ts)
  })
  
  baseline = data.frame(SSQ1$Baseline, SSQ2$Baseline)
  colnames(baseline) <- c("Baseline-Pre", "Baseline-Post")
  #print(baseline)
  
  large = data.frame(SSQ1$StaticLarge, SSQ2$StaticLarge)
  colnames(large) <- c("StaticLarge-Pre", "StaticLarge-Post")
  #print(large)
  
  small = data.frame(SSQ1$StaticSmall, SSQ2$StaticSmall)
  colnames(small) <- c("StaticSmall-Pre", "StaticSmall-Post")
  #print(small)
  
  print(tapply(pre$score_TS, pre$Condition, mean))
  print(tapply(post$score_TS, post$Condition, mean))
}

CompareWithSession = function(pre, post){
  SSQ1 = tapply(pre$score_TS, pre$Session, function(ts){
    return(ts)
  })
  SSQ2 = tapply(post$score_TS, post$Session, function(ts){
    return(ts)
  })

  session1 = data.frame(SSQ1$'1', SSQ2$'1')
  colnames(session1) <- c("Ses1-Pre", "Ses1-Post")
  #print(session1)
  
  session2 = data.frame(SSQ1$'2', SSQ2$'2')
  colnames(session2) <- c("Ses2-Pre", "Ses2-Post")
  #print(session2)
  
  session3 = data.frame(SSQ1$'3', SSQ2$'3')
  colnames(session3) <- c("Ses3-Pre", "Ses3-Post")
  #print(session3)
  
  print(tapply(pre$score_TS, pre$Session, mean))
  print(tapply(post$score_TS, post$Session, mean))
}

PreSSQ = read.table(file=paste(folderName, "PreSSQ.csv", sep=""), header=TRUE, sep = ",")
PostSSQ = read.table(file=paste(folderName, "PostSSQ.csv", sep=""), header=TRUE, sep = ",")

PreSSQ = ComputeSSQ(PreSSQ)
PostSSQ = ComputeSSQ(PostSSQ)

compareTable = data.frame(PreSSQ$Participant, PreSSQ$Session, PreSSQ$Condition, PreSSQ$score_TS, PostSSQ$score_TS)
colnames(compareTable) <- c("Participant", "Session", "Condition","Pre", "Post")

#print(compareTable)

#compareTable[compareTable$Participant == 1,]

#CompareWithCondition(PreSSQ, PostSSQ)
#CompareWithSession(PreSSQ, PostSSQ)

PostSSQ$Participant = as.factor(PostSSQ$Participant)
PostSSQ$Session = as.factor(PostSSQ$Session)
plot(score_TS~Condition,data=PostSSQ)
plot(score_TS~Session, data=PostSSQ)

# 1-way ANOVA, repeated measure
#result=aov(score_TS~Condition+Error(Participant),data=PostSSQ)
print(summary(aov(score_TS~Condition+Error(Participant/Condition),data=PostSSQ)))
print(summary(aov(score_TS~Session+Error(Participant/Session),data=PostSSQ)))

# 2-way ANOVA, repeated measure
# Use the commented one when the conditions and sessions are fully mapping in the participants
#result = aov(score_TS~Condition*Session+Error(Participant/(Condition*Session)),data=PostSSQ)
print(summary(aov(score_TS~Condition*Session+Error(Participant),data=PostSSQ)))

# linear mixed model
# H0: mA=mB=mC
library(nlme)
lmeResult1 = lme(score_TS~Condition+Session+Condition*Session,random=~1|Participant,data=PostSSQ,method="ML")
lmeResult2 = lme(score_TS~Condition+Session,random=~1|Participant,data=PostSSQ,method="ML")
lmeResult3 = lme(score_TS~Condition,random=~1|Participant,data=PostSSQ,method="ML")
lmeResult4 = lme(score_TS~Session,random=~1|Participant,data=PostSSQ,method="ML")
print(summary(lmeResult1))