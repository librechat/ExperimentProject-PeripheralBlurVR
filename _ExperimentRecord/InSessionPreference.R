folderName = "./Study1/"
Preference = read.table(file=paste(folderName, "Preference.csv", sep=""), header=TRUE, sep = ",")

Preference$Participant = as.factor(Preference$Participant)
Preference$Session = as.factor(Preference$Session)

plot(SmallAffordance~Condition,data=Preference)
plot(SmallAffordance~Session, data=Preference)

plot(EasyToSearch~Condition,data=Preference)
plot(EasyToSearch~Session, data=Preference)

plot(Comfortable~Condition,data=Preference)
plot(Comfortable~Session, data=Preference)

plot(GoodExp~Condition,data=Preference)
plot(GoodExp~Session, data=Preference)

TurnFormat = function(){
  write.csv(data.frame(
    group=Preference$Condition,
    value=Preference$SmallAffordance
  ),file="./Study1/OrganizedData/SmallAffordance_Condition.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Session,
    value=Preference$SmallAffordance
  ),file="./Study1/OrganizedData/SmallAffordance_Session.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Condition,
    value=Preference$EasyToSearch
  ),file="./Study1/OrganizedData/EasyToSearch_Condition.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Session,
    value=Preference$EasyToSearch
  ),file="./Study1/OrganizedData/EasyToSearch_Session.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Condition,
    value=Preference$Comfortable
  ),file="./Study1/OrganizedData/Comfortable_Condition.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Session,
    value=Preference$Comfortable
  ),file="./Study1/OrganizedData/Comfortable_Session.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Condition,
    value=Preference$GoodExp
  ),file="./Study1/OrganizedData/GoodExp_Condition.csv",row.names=FALSE)
  write.csv(data.frame(
    group=Preference$Session,
    value=Preference$GoodExp
  ),file="./Study1/OrganizedData/GoodExp_Session.csv",row.names=FALSE)
}