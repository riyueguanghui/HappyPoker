# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Mail(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.globalData["Mail"]=self
		self.mailData={}
		self.convertMainData()
		self.addTimer(15,15,0)

	#转换到局部数据
	def convertMainData(self):
		for mail in self.mailList:
			recvid=mail["recverID"]
			if recvid in self.mailData.keys():
				self.mailData[recvid].append(mail)
			else:
				self.mailData[recvid]=[mail]


	def onTimer(self,id,userArg):
		self.writeToDB(self.writeToDBcb,True)


	#回调函数
	def writeToDBcb(self,isSuccess,entity):
		pass

	#将要写进数据，同步mailData数据
	def onWriteToDB(self,cellData):
		self.mailList=[]
		for data in self.mailData.values():
			if data:
				self.mailList.extend(data)

	#请求保存邮件到数据库
	def saveMailToDB(self,mail):
		dbID=mail["recverID"]
		if dbID in self.mailData.keys():
			self.mailData[dbID].append(mail)
		else:
			self.mailData[dbID]=[mail]


	#删除邮件
	def deleteMailFromDB(self,senderID,databaseID):
		if databaseID in self.mailData.keys():
			list=self.mailData[databaseID]
			for mail in list:
				if mail["senderID"]==senderID:
					list.remove(mail)
			self.mailData[databaseID]=list


	#获取玩家邮件
	def getPlayerMail(self,entity,databaseID):
		if databaseID in self.mailData.keys():
			data=self.mailData[databaseID]
			for mail in data:
				entity.playerReceiveMail(mail)

