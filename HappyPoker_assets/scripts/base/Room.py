# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

class Room(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		self.createCellEntityInNewSpace(None)						#创建空间
		self.roomId=self.cellData["roomId"]							#保存roomId，base创建后会删除cellData


	#房间需要玩家的数量
	def needPlayerNum(self):
		if self.isDestroyed:
			return 0
		return 3-len(self.playerList)

	#添加玩家到房间里
	def addPlayerToRoom(self,entityCall):
		if entityCall not in self.playerList:
			self.playerList.append(entityCall)
		if(len(self.playerList)==3 and self.roomType==0):
			KBEngine.globalData["Hall"].roomNotNeedPlayer(self,self.roomId)		#如果满人，告诉大厅不需要玩家了
		if(len(self.playerList)==3 and self.roomType==1):
			entityCall.client.returnErrorMessage(3)									#如果满人，告诉客户端加入的房间已满人
		if self.cell is not None:
			self.cell.initPlayerCellData(entityCall.playerName,entityCall.playerBean,entityCall)								#初始化玩家的cell部分数据
			print("3333333333333333")
			print(entityCall.playerName)
			#self.addPlayerCellToRoom(entityCall)
	
	#删除玩家,告诉大厅需要玩家
	def delPlayerFromRoom(self,entityID):
		for entity in self.playerList:
			if entity.id==entityID:
				self.playerList.remove(entity)
		print("playerList length:")
		print(len(self.playerList))
		KBEngine.globalData["Hall"].roomNeedPlayer(self,self.roomId)

	#添加玩家Cell实体到房间
	def addPlayerCellToRoom(self,entityCall):
		if entityCall.cell is None:
			print("entityCall without cell!")
			entityCall.createPlayerCell(self.cell)
		else:
			print("entityCall had cell!")
			entityCall.onTeleport(self)
		
	#房间实体创建后调用，判断是否需要玩家
	def onGetCell(self):
		for playerEntity in self.playerList:									#新创的满人房间仍需要调用addPlayerToRoom,目的创建玩家的cell部分
			self.addPlayerToRoom(playerEntity)
		if len(self.playerList)<3 and self.roomType==0:
			KBEngine.globalData["Hall"].roomNeedPlayer(self,self.roomId)