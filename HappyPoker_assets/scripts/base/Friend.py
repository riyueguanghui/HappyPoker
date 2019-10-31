# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Friend(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.globalData["Friend"]=self
		self.publicData={}
		self.synchroData={}
		self.convertPublicData()
		self.addTimer(15,15,0)


	#转换到局部数据
	def convertPublicData(self):
		for data in self.publicDataList:
			dbID=data["playerID"]
			self.publicData[dbID]=data

		for data in self.synchroFriendList:
			dbID=data["senderID"]
			if dbID in synchroData.keys():
				synchroData[dbID].append(data)
			else:
				synchroData[dbID]=[data]


	#保存数据到数据库
	def onTimer(self,id,userArg):
		self.writeToDB(self.writeToDBcb,True)

	#回调函数
	def writeToDBcb(self,isSuccess,entity):
		pass


	#将要写进数据库，同步mailData数据
	def onWriteToDB(self,cellData):
		self.publicDataList=[]
		for data in self.publicData.values():
			if data:
				self.publicDataList.append(data)
		
		self.synchroFriendList=[]
		for data in self.synchroData.values():
			if data:
				self.synchroFriendList.extend(data)
		


	#玩家上线：更新数据&同步添加好友
	def playerOnline(self,databaseID,playerData,friendList):
		self.publicData[databaseID]=playerData
		#self.updateFriendStatus(1,friendList)

		
		if databaseID in self.synchroData.keys():
			dataList=self.synchroData[databaseID]
			for data in dataList:
				if data:
					playerData["playerEntity"].playerSynchroFriend(data["recverID"])
			self.synchroData.pop(databaseID)


	#玩家下线：删除数据
	def playerOffline(self,databaseID,friendList):
		self.publicData[databaseID]["status"]=0
		#self.updateFriendStatus(0,friendList)

	#通知好友我在线或离线线
	#status=1:在线 status=0:离线
	def updateFriendStatus(self,status,friendList):
		for dbID in friendList:
			if self.publicData[dbID]["status"]==1:
				entity=self.publicData[dbID]["entity"]
				entity.updatePlayerStatus(status)


	#获取玩家数据
	def getPlayerData(self,databaseID):
		return self.publicData[databaseID]

	#获取玩家好友列表数据
	def getPlayerListData(self,friendList):
		dataList=[]
		for dbID in friendList:
			if dbID in self.publicData.keys():
				dataList.append(self.publicData[dbID])
		return dataList


	#保存同步好友数据
	def saveSynchroFriend(self,senderID,recverID):
		data={
			"senderID":senderID,
			"recverID":recverID
		}
		if senderID in self.synchroData.keys():
			self.synchroData[senderID].append(data)
		else:
			self.synchroData[senderID]=[data]




