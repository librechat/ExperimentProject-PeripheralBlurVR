folderName = "./Study1/"
#TotalTime = read.table(file=paste(folderName, "TotalTime_RemovedNonTerminated.csv", sep=""), header=TRUE, sep = ",")
TotalTime = read.table(file=paste(folderName, "TotalTime.csv", sep=""), header=TRUE, sep = ",")

TotalTime$Participant = as.factor(TotalTime$Participant)
TotalTime$Session = as.factor(TotalTime$Session)
plot(Time~Condition,data=TotalTime)
plot(Time~Session, data=TotalTime)

# 1 way ANOVA
print(summary(aov(Time~Condition+Error(Participant/Condition),data=TotalTime)))
print(summary(aov(Time~Session+Error(Participant/Session),data=TotalTime)))

# 2 way ANOVA
print(summary(aov(Time~Condition*Session+Error(Participant),data=TotalTime)))

# normal distribution
qqnorm(TotalTime$Time)
qqline(TotalTime$Time)

print(shapiro.test(TotalTime$Time))