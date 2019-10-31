# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Account(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)


	#玩家离开房间
	def leaveRoom(self):
		KBEngine.globalData["Room_%i" % self.spaceID].leaveGameRoom(self)

	#玩家继续游戏
	def stayRoom(self,roomIndex):
		self.allClients.stayInRoom(roomIndex)

	#玩家数据，方便其他客户端调用
	def initPlayerData(self,entityData):
		self.playerData=entityData
		print("public data to load!")
	
	#是否准备游戏
	#-1：取消准备游戏
	#0：没有准备游戏
	#1：准备游戏
	def setGameReady(self,state):
		index=self.playerData["roomIndex"]
		self.allClients.playerReadyGame(index,state)
		KBEngine.globalData["Room_%i" % self.spaceID].updateReadyNum(state)



	#初始玩家扑克牌
	def initPlayerPoker(self,gamePoker):
		self.playerPoker=gamePoker
		self.client.initGamePoker()

	#更新打出去的牌
	def updatePlayerPoker(self,roomIndex,pokerList):
		print("update player poker")
		self.allClients.displayPoker(pokerList)
		for poker in pokerList:
			print(poker)
			self.playerPoker.remove(poker)

		if len(self.playerPoker)==0:
			self.allClients.pokerGameOver(roomIndex)
			KBEngine.globalData["Room_%i" %self.spaceID].playGameOver()
		else:
			KBEngine.globalData["Room_%i" % self.spaceID].nextToPlayPoker()

	#更新欢乐豆数量
	def updatePlayerBean(self,beans):
		self.base.updatePlayerBean(beans)


	#玩家过牌
	def passPlayerPoker(self):
		print("pass play poker")
		self.allClients.passPoker()


	#玩家抢地主
	#state==0:不抢地主
	#state==1:抢地主
	def playerMakeLandlord(self,index,state):
		self.allClients.playerToLandlord(index,state)
		KBEngine.globalData["Room_%i" % self.spaceID].updateLandlord(index,state)


	#添加三张地主牌并告诉客户端谁是地主
	def addLandlordPoker(self,landlord,gamePoker):
		self.playerPoker+=gamePoker
		self.client.initGamePoker()
		self.allClients.loadLandlordPoker(landlord,gamePoker)