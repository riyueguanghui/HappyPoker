# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Account(KBEngine.Proxy):
	def __init__(self):
		KBEngine.Proxy.__init__(self)
		if self.playerNew==1:							#新玩家，初始数据
			self.playerNew=0 							#设置为非新玩家
			self.playerBean=9999998 			    #初始欢乐豆数量
			self.playerId=self.databaseID 				#初始玩家Id
			self.playerName=self.__ACCOUNT_NAME__ 		#初始玩家昵称
		self.roomIndex=0 								#房间内玩家的座位号

	#玩家匹配游戏
	def playerMatchGame(self):
		print("player start to match game!")
		KBEngine.globalData["Hall"].matchGame(self)		#调用Hall实体的matchGame方法

	#玩家加入房间
	def playerJoinGame(self,roomId):
		print("player start ot join game!")
		KBEngine.globalData["Hall"].joinGame(roomId,self)		#调用Hall实体的joinGame方法

	#玩家创建房间
	def playerCreateGame(self,roomId):
		print("player start to create game!")
		KBEngine.globalData["Hall"].createGame(roomId,self)		#调用Hall实体的createGame方法

	#玩家离开房间
	def playerLeaveRoom(self):
		print("player start to leave room")
		self.destroyCellEntity()
	
	#创建玩家的Cell实体
	def createPlayerCell(self,cellEntityCall):
		if self.cell is not None:
			print("don't need create new cell!")
			return
		self.createCellEntity(cellEntityCall)


	#设置玩家房间内的座位号
	def setRoomIndex(self,roomIndex):
		self.roomIndex=roomIndex

	#暂时的传送功能
	def onTeleport(self,cellEntityCall):
		self.teleport(cellEntityCall)

	#初始化ALL_CLIENTS部分数据
	def loadPlayerData(self):
		playerData={
					"playerName":self.playerName,
					"playerBean":self.playerBean,
					"roomIndex":self.roomIndex,
					"playerID":self.databaseID
		}
		if self.cell:
			self.cell.initPlayerData(playerData)
		else:
			self.cellData["playerData"]=playerData

	#更新欢乐豆数量
	def updatePlayerBean(self,beans):
		self.playerBean=self.playerBean+beans


	#玩家发送邮件
	def playerSendMail(self,recvID):
		mail={
			"senderID":self.databaseID,
			"senderName":self.playerName,
			"recverID":recvID
		}

		KBEngine.globalData["Mail"].saveMailToDB(mail)
		playerData=KBEngine.globalData["Friend"].getPlayerData(recvID)
		if playerData["status"]==1:
			playerData["playerEntity"].playerReceiveMail(mail);

		


	#玩家接收邮件
	def playerReceiveMail(self,mailData):
		self.client.receiveMail(mailData)

	#请求邮件数据
	def reqPlayerMail(self):
		KBEngine.globalData["Mail"].getPlayerMail(self,self.databaseID)

	#好友是否在线
	def getFriendListData(self):
		dataList=KBEngine.globalData["Friend"].getPlayerListData(self.friendList)
		self.client.friendListData(dataList)

	#玩家是否同意通过好友添加
	#agree=1:同意 agree=0:拒绝
	def playerAgreeFriend(self,senderID,agree):
		if agree==1:
			if senderID not in self.friendList:
				self.friendList.append(senderID)
			playerData=KBEngine.globalData["Friend"].getPlayerData(senderID)
			if playerData["status"]==1:
				playerData["playerEntity"].playerSynchroFriend(self.databaseID)
			else:
				KBEngine.globalData["Friend"].saveSynchroFriend(senderID,self.databaseID)
		KBEngine.globalData["Mail"].deleteMailFromDB(senderID,self.databaseID)

		

	#双方同步添加好友
	def playerSynchroFriend(self,recverID):
		if recverID not in self.friendList:
			self.friendList.append(recverID)
		print(recverID)
		print("synchro friend data!")




	def onTimer(self, id, userArg):
		"""
		KBEngine method.
		使用addTimer后， 当时间到达则该接口被调用
		@param id		: addTimer 的返回值ID
		@param userArg	: addTimer 最后一个参数所给入的数据
		"""
		DEBUG_MSG(id, userArg)
		
	def onClientEnabled(self):
		"""
		KBEngine method.
		该entity被正式激活为可使用， 此时entity已经建立了client对应实体， 可以在此创建它的
		cell部分。
		"""
		INFO_MSG("account[%i] entities enable. entityCall:%s" % (self.id, self.client))
		
		playerData={
					"status":1,
					"playerID":self.databaseID,
					"playerName":self.playerName,
					"playerBean":self.playerBean,
					"playerEntity":self
		}
		KBEngine.globalData["Friend"].playerOnline(self.databaseID,playerData,self.friendList)
		#KBEngine.globalData["Mail"].playerReceiveMail(self,self.databaseID)
			
	def onLogOnAttempt(self, ip, port, password):
		"""
		KBEngine method.
		客户端登陆失败时会回调到这里
		"""
		INFO_MSG(ip, port, password)
		return KBEngine.LOG_ON_ACCEPT
		
	def onClientDeath(self):
		"""
		KBEngine method.
		客户端对应实体已经销毁
		"""
		DEBUG_MSG("Account[%i].onClientDeath:" % self.id)
		KBEngine.globalData["Friend"].playerOffline(self.playerId,self.friendList)
		#self.destroy()
