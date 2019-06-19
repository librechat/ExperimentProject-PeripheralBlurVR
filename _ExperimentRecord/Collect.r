folderName = "./Study1_Pilot2/Collect/"
temp = list.files(path=folderName ,pattern="*.csv")

conditionNames = c("StaticSmall","StaticLarge","Baseline")
sessionNames = c("Session1","Session2","Session3")

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
CompareCollect()