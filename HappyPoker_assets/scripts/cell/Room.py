# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import random
from queue import Queue

class Room(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.addSpaceGeometryMapping(self.spaceID, None, "spaces/gameMap")	#告诉客户端加载地图
		KBEngine.globalData["Room_%i" % self.spaceID] = self
		self.playerEntityList=[0,0,0]											#玩家列表
		self.landlordPoker=[] 													#地主牌
		self.electQueue=Queue(3)												#叫地主玩家队列
		self.funcDict={}														#函数的字典映射
		self.lastPlayPokerData={"poker":0,"type":0,"count":0} 					#上一个玩家打牌的数据：卡牌和类型
		self.round=0
		self.landlord=0 														#地主
		self.nextPlayer=0 														#轮到谁出牌
		self.readyNum=0 														#已准备游戏的玩家数量
		self.lastPlayer=0 														#上一个出牌的玩家


	#初始化玩家cell实体数据，方便写游戏逻辑
	def initPlayerCellData(self,playerName,playerBean,entityCall):
		roomIndex=0
		for i in range(3):
			if self.playerEntityList[i]==0:
				self.playerEntityList[i]=entityCall
				roomIndex=i
				break

		entityCall.setRoomIndex(roomIndex)
		entityCall.loadPlayerData()
		self.base.addPlayerCellToRoom(entityCall)								#告诉房间可以添加玩家的Cell到空间了
		print("player go into the roomId is %d" %(self.roomId))

	#玩家离开房间
	def leaveGameRoom(self,entityCall):
		for i in range(3):
			if self.playerEntityList[i]!=0 and self.playerEntityList[i].id==entityCall.id:
				self.playerEntityList[i]=0

		entityCall.base.playerLeaveRoom()
		self.base.delPlayerFromRoom(entityCall.id)




	#更新准备游戏玩家的数量
	def updateReadyNum(self,state):
		self.readyNum+=state
		if self.readyNum==3:

			#开始发牌
			self.distributePoker()
	

	#更新地主玩家
	def updateLandlord(self,index,state):
		if self.round<3:
			if state==1:
				self.gameMupe*=2
				self.landlord=index
				self.electQueue.put(index)

			self.round+=1
			if self.round==3 and self.electQueue.qsize()<2:
				self.startToPlayGame()

			else:
				i=self.electQueue.get()
				self.makeLandlord(i)
				
		else:
			if state==1:
				self.gameMupe*=2
				self.landlord=index
				self.startToPlayGame()
			elif self.electQueue.qsize()==2:
				i=self.electQueue.get()
				self.makeLandlord(i)
			else:
				self.landlord=self.electQueue.get()
				self.startToPlayGame()



	#抢地主
	def makeLandlord(self,index):
		self.playerEntityList[index].client.notifyToCallLandlord()


	#分发卡牌
	def distributePoker(self):
		poker=[]
		for i in range(12,60):
			poker.append(i)
		for i in range(68,72):
			poker.append(i)
		poker.append(76)
		poker.append(80)
		random.shuffle(poker)

		playerGamePoker={}
		playerGamePoker[self.playerEntityList[0]]=poker[0:17]
		playerGamePoker[self.playerEntityList[1]]=poker[17:34]
		playerGamePoker[self.playerEntityList[2]]=poker[34:51]
		self.landlordPoker=poker[51:54]
		for entity in self.playerEntityList:
			pokerList=playerGamePoker[entity]
			entity.cell.initPlayerPoker(pokerList)

		#发完牌开始抢地主
		self.landlord=random.randint(0,2)
		self.electQueue.put((self.landlord+1)%3);
		self.electQueue.put((self.landlord+2)%3);
		self.makeLandlord(self.landlord)





	########################################################################################################
	#单张牌
	def onePoker(self,pokerList):
		data={
			"poker":pokerList[0]//4,
			"type":"DDZ_DP",
			"count":1
		}
		return data

	#两张牌
		#王炸
		#对子
	def twoPoker(self,pokerList):
		#王炸
		if 76 in pokerList and 80 in pokerList:
			data={
				"poker":19,
				"type":"DDZ_WZ",
				"count":2
			}
			return data

		#对子
		if pokerList[0]//4==pokerList[1]//4:
			data={
				"poker":pokerList[0]//4,
				"type":"DDZ_DZ",
				"count":2
			}
			return data

		#牌型错误
		data={}
		return data

	#三张牌
		#三张
	def threePoker(self,pokerList):
		if pokerList[0]//4==pokerList[1]//4 and pokerList[0]//4==pokerList[2]//4:
			data={
				"poker":pokerList[0]//4,
				"type":"DDZ_SD",
				"count":3
			}
			return data
		
		#牌型错误
		data={}
		return data


	#四张牌
		#炸弹
		#三带一
	def fourPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#炸弹
		if len(dict)==1:
			data={
				"poker":pokerList[0]//4,
				"type":"DDZ_ZD",
				"count":4
			}
			return data

		#三带一
		if len(dict)==2:
			for key,value in dict.items():
				if value==3:
					data={
						"poker":key,
						"type":"DDZ_SDY",
						"count":4
					}
					return data

		#牌型错误
		data={}
		return data

	#五张牌
		#单顺子
		#三带二
	def fivePoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#三带二
		if len(dict)==2 and 3 in dict.values():
			for key,value in dict.items():
				if value==3:
					data={
						"poker":key,
						"type":"DDZ_SDE",
						"count":5
					}
					return data

		#顺子
		if len(dict)==5:
			list=sorted(dict.keys())
			for i in range(1,5):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":5
			}
			return data

		#排序错误
		data={}
		return data


	#六张牌
		#顺子
		#连对
		#三张
		#四带二
	def sixPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#三张
		if len(dict)==2 and 3 in dict.values():
			list=sorted(dict.keys())
			if list[1]-list[0]!=1:
				data={}
				return data

			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":6
			}
			return data

		#四带二
		if len(dict)==2 and 4 in dict.values():
			for key,value in dict.items():
				if value==4:
					data={
						"poker":key,
						"type":"DDZ_SDE",
						"count":6
					}
					return data

		#连对
		if len(dict)==3:
			for v in dict.values():
				if v!=2:
					data={}
					return data

			list=sorted(dict.keys())
			if list[1]-list[0]!=1 or list[2]-list[1]!=1:
				data={}
				return data

			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":6
			}
			return data

		#顺子
		if len(dict)==6:
			list=sorted(dict.keys())
			for i in range(1,6):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":6
			}
			return data

		#排序错误
		data={}
		return data


	#七张牌
		#单顺子
	def sevenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#顺子
		if len(dict)==7:
			list=sorted(dict.keys())
			for i in range(1,7):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":7
			}
			return data

		#牌型错误
		data={}
		return data


	#八张牌
		#单顺子
		#连对
		#四带两对
		#飞机（2个三带一）
	def eightPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#四带两对
		if len(dict)==3 and 3 not in dict.values():
			if 4 in dict.values() and 2 in dict.values():
				for key,value in dict.items():
					if value==4:
						data={
							"poker":key,
							"type":"DDZ_SDLD",
							"count":8
						}
						return data
			#牌型错误
			data={}
			return data

		#连对
		if len(dict)==4 and 2 in dict.values():
			if 12 in dict.values():
				data={}
				return data

			list=sorted(dict.keys())
			for i in range(1,4):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=2:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":8
			}
			return data


		#飞机
		if len(dict)<5 and 3 in dict.values():
			n=0
			k=0
			for key,values in dict.items():
				if values==3:
					n+=1
					if n==1:
						k=key
				if n==2:
					if key-k==1 or k-key==1:
						data={
							"poker":key,
							"type":"DDZ_FJ",
							"count":8
						}
						return data

			#牌型错误
			data={}
			return data


		#顺子
		if len(dict)==8:
			list=sorted(dict.keys())
			for i in range(1,8):
				if list[i]-list[i-1]!=1:
					data={}
					return data

			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":8
			}
			return data


		#牌型错误
		data={}
		return data



	#九张牌
		#顺子
		#三张（3对）
	def ninePoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#三张
		if len(dict)==3:
			list=sorted(dict.keys())
			for i in range(1,3):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=3:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":9
			}
			return data

		#顺子
		if len(dict)==9:
			list=sorted(dict.keys())
			for i in range(1,9):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":9
			}
			return data

		#牌型错误
		data={}
		return data


	#十张牌
		#顺子
		#连对
		#飞机（2个三带二）
	def tenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#飞机
		if 3 in dict.values():
			if len(dict)==3 and 4 in dict.values():
				list=[]
				for key,value in dict.items():
					if value==3:
						list.append(key)
				if abs(list[1]-list[0]!=1):
					data={}
					return data

				data={
					"poker":list[0],
					"type":"DDZ_FJI",
					"count":10
				}
				return data

			if len(dict)==4 and 2 in dict.values():
				n=0
				list=[]
				for key,value in dict.items():
					if value==2:
						n+=1
					elif value==3:
						list.append(key)
				if n!=2 or abs(list[1]-list[0])!=1:
					data={}
					return data

				data={
					"poker":list[0],
					"type":"DDZ_FJ",
					"count":10
				}
				return 	data

			#牌型错误
			data={}
			return data


		#连对
		if len(dict)==5:
			list=sorted(dict.keys())
			for i in range(1,5):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=2:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":10
			}
			return data

		#顺子
		if len(dict)==10:
			list=sorted(dict.keys())
			for i in range(1,10):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":10
			}
			return data

		#牌型错误
		data={}
		return data
		

	#十一张牌
		#顺子
	def elevenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		if len(dict)==11:
			list=sorted(dict.keys())
			for i in range(1,11):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":11
			}
			return data

		#牌型错误
		data={}
		return data

	#十二张牌
		#顺子
		#连对（6个对子）
		#三张（4个三张）
		#飞机（3个三带一）
	def twelvePoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#三张
		if len(dict)==4:
			list=sorted(pokerList.keys())
			for i in range(1,4):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=3:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":12
			}
			return data

		#连对
		if len(dict)==6:
			list=sorted(dict.keys())
			for i in range(1,6):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=2:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":12
			}
			return data

		#连对
		if len(dict)>4 and len(dict)<7:
			list=[]
			for key,value in dict.items():
				if value==3:
					list.append(key)
			if len(list)!=3:
				data={}
				return data

			list.sort()
			for i in range(1,3):
				if list[i]-list[i-1]!=1:
					data={}
					return data

			data={
				"poker":list[0],
				"type":"DDZ_FJ",
				"count":12
			}
			return data


		#顺子
		if len(dict)==12:
			list=sorted(dict.keys())
			for i in range(1,12):
				if list[i]-list[i-1]!=1:
					data={}
					return data;
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":12
			}
			return data

		#牌型错误
		data={}
		return data

	#十三张牌
	def thirteenPoker(self,pokerList):
		data={}
		return data

	#十四张牌
		#连对（7个二对子）
	def fourteenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1
		if len(dict)==7:
			list=sorted(dict.keys())
			for i in range(1,7):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=2:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":14
			}
			return data

		#牌型错误
		data={}
		return data
	

	#十五张牌
		#三张（5个三张）
		#飞机（3个三带二）
	def fifteenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#飞机（炸弹作为对子）
		if len(dict)==5 and 4 in dict.values():
			list=[]
			for key,value in dict.items():
				if value==3:
					list.append(value)

			if len(list)!=3:
				data={}
				return data

			list.sort()
			for i in range(1,3):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_FJ",
				"count":15
			}
			return data

		#飞机
		if len(dict)==6:
			n=0
			list=[]
			for key,value in dict.items():
				if value==2:
					n+=1
				elif value==3:
					list.append(key)

			if n!=3:
				data={}
				return data

			list.sort()
			for i in range(1,3):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_FJ",
				"count":15
			}
			return data


		#三张
		if len(dict)==5:
			list=sorted(dict.keys())
			for i in range(1,5):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=3:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_SZ",
				"count":15
			}
			return data

		#牌型错误
		data={}
		return data


	#十六张牌
		#连对（8个对子）
		#飞机（4个三带一）
	def sixteenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#飞机
		if len(dict)==8 and 3 in dict.values():
			list=[]
			for key,value in dict.items():
				if value==3:
					list.append(key)

			if len(list)<4:
				data={}
				return data

			for i in range(1,4):
				if list[i]-list[i-1]!=1:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_FJ",
				"count":16
			}
			return data

		#连对
		if len(dict)==8:
			list=sorted(dict.keys())
			for i in range(1,8):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=2:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":16
			}
			return data

		#牌型错误
		data={}
		return data



	#十七张牌
	def seventeenPoker(self,pokerList):
		data={}
		return data

	#十八张牌
		#连对（9个对子）
		#三张（6个三张）
	def eighteenPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=dict.get(i,0)+1

		#三张
		if len(dict)==6:
			list=sorted(dict.keys())
			for i in range(1,6):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=3:
					data={}
					return data
			data={
				"poker":lsit[5],
				"type":"DDZ_SZ",
				"count":18
			}
			return data

		#连对
		if len(dict)==9:
			list=sorted(dict.keys())
			for i in range(1,9):
				if list[i]-list[i-1]!=1 or dict[list[i]]!=2:
					data={}
					return data
			data={
				"poker":list[0],
				"type":"DDZ_LD",
				"count":18
			}
			return data

		#牌型错误
		data={}
		return data



	#十九张牌
	def nineteenPoker(self,pokerList):
		data={}
		return data

	#二十张牌
		#连对（10个对子）
		#飞机（5个三带一）
		#飞机（4个三带二）
	def twentyPoker(self,pokerList):
		dict={}
		for i in pokerList:
			i=i//4
			dict[i]=get(i,0)+1

		four=0
		twoList=[]
		threeList=[]
		for key,value in dict.items():
			if value==2:
				twoList.append(key)
			elif value==3:
				threeList.append(key)
			elif value==4:
				four+=1

		#连对
		if len(twoList)==10:
			twoList.sort()
			for i in range(1,20):
				if twoList[i]-twoList[i-1]!=1:
					data={}
					return data
			data={
				"poker":twoList[0],
				"type":"DDZ_LD",
				"count":20
			}
			return data

		#飞机（5个三带一）
		if len(threeList)==5:
			threeList.sort()
			for i in range(1,5):
				if threeList[i]-threeList[i-1]!=1:
					data={}
					return data
			data={
				"poker":threeList[4],
				"type":"DDZ_FJ",
				"count":20
			}
			return data

		#飞机（4个三代二）
		if len(threeList)==4:
			threeList.sort()
			if (four*4+len(twoList)*2)!=8:
				data={}
				return data

			for i in range(1,4):
				if threeList[i]-threeList[i-1]!=1:
					data={}
					return data
			data={
				"poker":threeList[3],
				"type":"DDZ_FJ",
				"count":20,
			}
			return data

		#牌型错误
		data={}
		return data
	###########################################################################################################


	#开始游戏
	def startToPlayGame(self):
		print("shtart to play game!")
		self.nextPlayer=self.landlord
		self.lastPlayer=self.landlord
		self.playerEntityList[self.landlord].cell.addLandlordPoker(self.landlord,self.landlordPoker)
		dict={
			1:self.onePoker,
			2:self.twoPoker,
			3:self.threePoker,
			4:self.fourPoker,
			5:self.fivePoker,
			6:self.sixPoker,
			7:self.sevenPoker,
			8:self.eightPoker,
			9:self.ninePoker,
			10:self.tenPoker,
			11:self.elevenPoker,
			12:self.twelvePoker,
			13:self.thirteenPoker,
			14:self.fourteenPoker,
			15:self.fifteenPoker,
			16:self.sixteenPoker,
			17:self.seventeenPoker,
			18:self.eighteenPoker,
			19:self.nineteenPoker,
			20:self.twentyPoker
		}
		self.funcDict=dict
		self.playerEntityList[self.landlord].client.notifyToPlayPoker(0)


	#玩家出牌
	def playPoker(self,pokerList):
		num=len(pokerList)
		data=self.funcDict.get(num)(pokerList)
		if len(data)==0:
			print("data is none")
			self.playerEntityList[self.nextPlayer].client.playPokerFailed()
			return

		if self.nextPlayer==self.lastPlayer:
			self.lastPlayPokerData=data
			if data["type"]=="DDZ_WZ" or data["type"]=="DDZ_ZD":
				self.gameMupe*=2
				print("gameMupe 0")

			self.playerEntityList[self.nextPlayer].cell.updatePlayerPoker(self.nextPlayer,pokerList)
		else:
			if data["type"]=="DDZ_WZ":															#王炸
				self.gameMupe*=2
				print("gameMupe *")
				self.lastPlayPokerData=data
				self.lastPlayer=self.nextPlayer
				self.playerEntityList[self.nextPlayer].cell.updatePlayerPoker(self.nextPlayer,pokerList)
			elif data["type"]=="DDZ_ZD" and self.lastPlayPokerData["type"]!="DDZ_ZD" and self.lastPlayPokerData["type"]!="DDZ_WZ":		#炸弹
				self.gameMupe*=2
				print("gameMupe **")
				self.lastPlayPokerData=data
				self.lastPlayer=self.nextPlayer
				self.playerEntityList[self.nextPlayer].cell.updatePlayerPoker(self.nextPlayer,pokerList)
			elif data["poker"]>self.lastPlayPokerData["poker"] and data["type"]==self.lastPlayPokerData["type"] and data["count"]==self.lastPlayPokerData["count"]:
				self.lastPlayPokerData=data
				self.lastPlayer=self.nextPlayer
				self.playerEntityList[self.nextPlayer].cell.updatePlayerPoker(self.nextPlayer,pokerList)
			else:
				print("data is error")
				self.playerEntityList[self.nextPlayer].client.playPokerFailed()

	#玩家不出牌
	def unPlayPoker(self):
		self.playerEntityList[self.nextPlayer].cell.passPlayerPoker()
		self.nextToPlayPoker()
		

	#下一个玩家出牌
	def nextToPlayPoker(self):
		self.nextPlayer=(self.nextPlayer+1)%3
		if self.nextPlayer==self.lastPlayer:
			print("equite")
			self.playerEntityList[self.nextPlayer].client.notifyToPlayPoker(0)
		else:
			print("not equite")
			self.playerEntityList[self.nextPlayer].client.notifyToPlayPoker(1)


	#游戏结束，清除房间数据
	def cleanRoomData(self):
		self.landlordPoker=[] 													#地主牌
		self.electQueue=Queue(3)												#叫地主玩家队列
		self.lastPlayPokerData={"poker":0,"type":0,"count":0} 					#上一个玩家打牌的数据：卡牌和类型
		self.round=0
		self.landlord=0 														#地主
		self.nextPlayer=0 														#轮到谁出牌
		self.readyNum=0 														#已准备游戏的玩家数量
		self.lastPlayer=0 														#上一个出牌的玩家
		self.gameMupe=15														#倍数



	#游戏结束
	def playGameOver(self):
		beans=20*self.gameMupe
		if self.landlord==self.nextPlayer:
			if self.landlord==0:
				self.playerEntityList[0].cell.updatePlayerBean(2*beans)
				self.playerEntityList[1].cell.updatePlayerBean(-1*beans)
				self.playerEntityList[2].cell.updatePlayerBean(-1*beans)
			elif self.landlord==1:
				self.playerEntityList[1].cell.updatePlayerBean(2*beans)
				self.playerEntityList[0].cell.updatePlayerBean(-1*beans)
				self.playerEntityList[2].cell.updatePlayerBean(-1*beans)
			else:
				self.playerEntityList[2].cell.updatePlayerBean(2*beans)
				self.playerEntityList[0].cell.updatePlayerBean(-1*beans)
				self.playerEntityList[1].cell.updatePlayerBean(-1*beans)

		else:
			if self.landlord==0:
				self.playerEntityList[0].cell.updatePlayerBean(-2*beans)
				self.playerEntityList[1].cell.updatePlayerBean(beans)
				self.playerEntityList[2].cell.updatePlayerBean(beans)
			elif self.landlord==1:
				self.playerEntityList[1].cell.updatePlayerBean(-2*beans)
				self.playerEntityList[0].cell.updatePlayerBean(beans)
				self.playerEntityList[2].cell.updatePlayerBean(beans)
			else:
				self.playerEntityList[2].cell.updatePlayerBean(-2*beans)
				self.playerEntityList[0].cell.updatePlayerBean(beans)
				self.playerEntityList[1].cell.updatePlayerBean(beans)

		self.cleanRoomData()



	
