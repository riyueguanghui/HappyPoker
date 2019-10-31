# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import Functor

class Hall(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.globalData["Hall"]=self  				#全局变量，方便调用
		self.matchGameTimer=0							#匹配游戏定时器
		self.matchPlayerEntity=[]						#匹配中的玩家实体：列表
		self.needPlayerRoomEntity={}					#需要玩家的房间实体：字典
		self.privateRoomEntity={}						#玩家创建的私立房间
		self.allRoomEntity={}							#所有房间实体：字典

	#玩家匹配游戏
	def matchGame(self,entityCall):
		print("successful to match game!")
		self.matchPlayerEntity.append(entityCall)		#把玩家实体添加到匹配游戏列表
		if self.matchGameTimer==0:						#判断定时器是否以及开始
			self.matchGameTimer=self.addTimer(0,0.5,4)	#添加定时器：0秒后开始执行，每0.5秒执行一次


	#玩家加入房间
	def joinGame(self,roomId,entityCall):
		if roomId in self.privateRoomEntity:
			roomEntity=self.privateRoomEntity[roomId]
			roomEntity.addPlayerToRoom(entityCall)
		else:
			print("system return the error message!")
			entityCall.client.returnErrorMessage(0)			#返回错误信息：房间号不存在

	#玩家创建房间
	def createGame(self,roomId,entityCall):
		if roomId not in self.privateRoomEntity:
			self.playerCreateRoom(roomId,entityCall)
		else:
			print("system return the error message")
			entityCall.client.returnErrorMessage(1)			#返回错误信息：房间已被创建

	#房间需要玩家
	def roomNeedPlayer(self,entityCall,roomId):
		if roomId not in self.needPlayerRoomEntity:
			self.needPlayerRoomEntity[roomId]=entityCall
		for id in self.needPlayerRoomEntity:
			print("need room id:")
			print(id)

	#房间不需要玩家了
	def roomNotNeedPlayer(self,entityCall,roomId):
		if roomId in self.needPlayerRoomEntity:
			self.needPlayerRoomEntity.pop(roomId)

	#玩家离开房间
	def leaveRoom(self,entityCall):
		pass
	
	#给玩家分配游戏房间
	def assignRoom(self):
		playerNum=len(self.matchPlayerEntity)
		if playerNum>0:
			for roomEntity in self.needPlayerRoomEntity.values():
				print(roomEntity.needPlayerNum())
				for i in range(roomEntity.needPlayerNum()):
					roomEntity.addPlayerToRoom(self.matchPlayerEntity.pop(0))
					playerNum-=1
					if(playerNum==0):
						return

			if playerNum>=3:
				for i in range(playerNum%3):
					self.systemCreateRoom(3)
					playerNum-=3

			if playerNum>0:
				self.systemCreateRoom(playerNum)
				playerNum=0


	#系统自动创建房间
	def systemCreateRoom(self,playerNum):
		playerEntityList=[]
		for i in range(playerNum):
			playerEntityList.append(self.matchPlayerEntity.pop(0))

		roomId=KBEngine.genUUID64()										#服务器自动生成64位的ID
		while roomId in self.allRoomEntity:								#检测房间ID是否重复
			roomId=KBEngine.genUUID64()

		#创建房间实体
		KBEngine.createEntityAnywhere("Room",{"roomId":roomId,"roomType":0,"playerList":playerEntityList},Functor.Functor(self.systemCreateRoomCB,roomId))


	#玩家创建房间
	def playerCreateRoom(self,roomId,entityCall):
		playerEntityList=[]
		playerEntityList.append(entityCall)
		KBEngine.createEntityAnywhere("Room",{"roomId":roomId,"roomType":1,"playerList":playerEntityList},Functor.Functor(self.playerCreateRoomCB,roomId))

	#系统创建房间成功回调此函数:房间添加到房间实体列表
	def systemCreateRoomCB(self,roomId,entityCall):
		self.allRoomEntity[roomId]=entityCall

	#玩家创建房间成功回调此函数：房间添加到私立房间实体列表
	def playerCreateRoomCB(self,roomId,entityCall):
		self.privateRoomEntity[roomId]=entityCall

	#定时器回调函数：不断分配房间
	def onTimer(self, id, userArg):
		self.assignRoom()
