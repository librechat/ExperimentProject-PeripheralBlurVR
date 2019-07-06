folderName = "./Study1/FinalCompare/"
Comparison = read.table(file=paste(folderName, "Comparison.csv", sep=""), header=TRUE, sep = ",")
Comparison$Session = as.factor(Comparison$Session)
Comparison$Participant = as.factor(Comparison$Participant)

plot(Comfortable~Condition, data=Comparison)
plot(Comfortable~Session, data=Comparison)

print(summary(aov(Comfortable~Condition+Error(Participant/Condition),data=Comparison)))
print(summary(lme(Comfortable~Condition,random=~1|Participant,data=Comparison,method="ML")))

plot(ImageQuality~Condition, data=Comparison)
plot(ImageQuality~Session, data=Comparison)

print(summary(aov(ImageQuality~Condition+Error(Participant/Condition),data=Comparison)))
print(summary(lme(ImageQuality~Condition,random=~1|Participant,data=Comparison,method="ML")))

plot(Preference~Condition, data=Comparison)
plot(Preference~Session, data=Comparison)

print(summary(aov(Preference~Condition+Error(Participant/Condition),data=Comparison)))
print(summary(lme(Preference~Condition,random=~1|Participant,data=Comparison,method="ML")))