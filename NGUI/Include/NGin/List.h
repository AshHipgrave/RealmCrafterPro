//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
#pragma once

namespace NGin
{
	//! Linked List Class
	/*!
		This class stores a collection items in a linked list for fast iteration.
		Use the provided foreachc() and nextc() macros to easily setup the list.
		The ClassDecl and ClassDef macros make is easier to have classes automatically register themselves
		with the list.
	*/
	template <class T>
	class List
	{
	private:

		struct ListNode
		{
			ListNode* Next;
			ListNode* Previous;
			T Node;

			ListNode() : Next(NULL), Previous(NULL) {};
		};

	public:

		class Iterator
		{
		public:
			
			ListNode* Current;

			Iterator() : Current(NullNode) {};
			Iterator(ListNode* Start) : Current(Start) {};

			Iterator& operator ++() { Current = Current->Next; return *this; };
			Iterator& operator --() { Current = Current->Previous; return *this; };
			Iterator operator ++(int) { Iterator Tmp = *this; Current = Current->Next; return Tmp; };
			Iterator operator --(int) { Iterator Tmp = *this; Current = Current->Previous; return Tmp; };

			Iterator operator+(int Num) const
			{
				Iterator Tmp = *this;

				if (Num >= 0)
					while (num-- && Tmp.Current != 0) ++Tmp;
				else
					while (num++ && Tmp.Current != 0) --Tmp;

				return Tmp;
			}

			Iterator& operator+=(int Num)
			{
				if (Num >= 0)
					while (Num-- && this->Current != 0) ++(*this);
				else
					while (Num++ && this->Current != 0) --(*this);

				return *this;
			}

			Iterator operator  -(int Num) const { return (*this)+(-Num) }
			Iterator operator -=(int Num) const { (*this)+=(-Num); return *this; }

			bool operator ==(const Iterator& Other) const { return Current == Other.Current; };
			bool operator !=(const Iterator& Other) const { return Current != Other.Current; };

			T& operator *() { return Current->Node; };


		};


		ListNode* First;
		ListNode* Last;
		ListNode* NullNode;
		int Size;

		List()
		{
			Size = 0;
			First = NULL;
			Last = NULL;
			NullNode = new ListNode();
			NullNode->Node = 0;
		}

		~List()
		{
			Clear();
			delete NullNode;
		}

		//! Creates and iterator at the start of the list.
		/*!
		\return And iterator to move through the list.
		*/
		virtual Iterator Begin()
		{
			if(First == 0)
				return Iterator(NullNode);
			else
				return Iterator(First);
		}

		//! Creates an iterator at the end of the list
		/*!
		\return An iterator to move through the list.
		*/
		virtual Iterator End()
		{
			if(Last == 0)
				return Iterator(NullNode);
			else
				return Iterator(Last);
		}

		//! Add an item to the list.
		/*!
		\param Item The item to be added to the list.
		\param AddAtStart Set to true if you want the item to be added to the end of the list.
		*/
		virtual void Add(T Item, bool AddAtStart)
		{
			if(AddAtStart == false)
				Add(Item);

			ListNode* Node = new ListNode();
			Node->Node = Item;

			if(Last == NULL)
				Last = Node;

			if(First != NULL)
			{
					Node->Previous = 0;
					Node->Next = First;
			}

			++Size;
			First = Node;
		}

		//! Add an item to the list.
		/*!
		\param Item The item to be added to the list.
		*/
		virtual void Add(T Item)
		{
			ListNode* Node = new ListNode();
			Node->Node = Item;
			
			if(First == NULL)
				First = Node;
			
			if(Last != NULL)
			{
				Node->Previous = Last;
				Last->Next = Node;
			}

			++Size;
			Last = Node;
		}

			//! Insert an item somewhere into the list
			/*!
			\param Item Item to add
			\param Index Index at which to add the item
			*/
			virtual void Insert(T Item, int Index)
			{
				if(Index == 0)
				{
					Add(Item, true);
					return;
				}

				ListNode* Split = First;
				for(int i = 1; i < Index; ++i)
				{
					Split = First->Next;
					if(Split == 0)
					{
						Add(Item);
						return;
					}
				}

				if(Split->Next == 0)
				{
					Add(Item);
					return;
				}

				ListNode* New = new ListNode();
				New->Node = Item;

				New->Next = Split->Next;
				New->Next->Previous = New;
				
				New->Previous = Split;
				Split->Next = New;

				++Size;
			}

		//! Remove an item from the list.
		/*!
		\param It Iterator at the current position to remove.
		*/
		virtual void Remove(Iterator& It)
		{
			ListNode* Item = It.Current;

			if(First == It.Current)
				First = It.Current->Next;

			if(Last == It.Current)
				Last = It.Current->Previous;

			if(It.Current->Next != NULL)
				if(It.Current->Previous != NULL)
				{
					It.Current->Next->Previous = It.Current->Previous;
					It.Current->Previous->Next = It.Current->Next;
				}else
					It.Current->Next->Previous = NULL;
			else if(It.Current->Previous != NULL)
				It.Current->Previous->Next = NULL;

			delete It.Current;
			//It.Current = 0;
			--Size;
		}

		
		//! Remove an item from the list.
		/*!
		\param Item The item in the list to remove.
		*/
		virtual bool Remove(T Item)
		{
			if(Count() == 0)
				return false;

			bool Found = false;
			Iterator It = Begin();

			for(;Count() > 0;++It)
			{
				if(Item == (*It))
				{
					Remove(It);
					Found = true;
					break;
				}
				if(It == End())
					break;
			}

			return Found;
		}

		//! Check if an item is in the list
		/*!
		\param Item The item to search for.
		*/
		virtual bool Find(T Item)
		{
			Iterator It = Begin();
			for(;Count() > 0; ++It)
			{
				if(Item == (*It))
					return true;
				if(It == End())
					return false;
			}
			return false;
		}

		//! Empty the list.
		virtual void Clear()
		{
			ListNode* Item = First;
			ListNode* Next;

			while(Item != NULL)
			{
				Next = Item->Next;
				delete Item;
				Item = Next;
			}

			Size = 0;
			First = NULL;
			Last = NULL;
		}

		//! Retrieve the number of objects in the list
		/*!
		\return Number of list items.
		*/
		virtual int Count()
		{
			return Size;
		}

		//! Retrieve the item before the particular item.
		/*!
		\param Item The current item.
		\return The item before the Item argument in the list.
		*/
		virtual T Before(T Item)
		{
			T LastSearched = 0;
			Iterator It = Begin();
			for(;;++It)
			{
				if(Item == (*It))
				{
					return LastSearched;
					break;
				}
				LastSearched = (*It);
				if(It == End())
					break;
			}
			return LastSearched;
		}

		
		//! Retrieve the item after the particular item.
		/*!
		\param Item The current item.
		\return The item after the Item argument in the list.
		*/
		virtual T After(T Item)
		{
				bool ReturnNext = false;
			Iterator It = Begin();
			for(;;++It)
			{
					if(ReturnNext)
					return *It;

				if(Item == (*It))
						ReturnNext = true;

				if(It == End())
					break;
			}
			return 0;
		}

	};
}

// foreach and next macros for moving through the list.
// These particular ones are designed for classes that internally register themselves (see ClassDecl/Def)
// Usage: foreach/nextc(IteratorName, ClassName, ListName)
#define foreachc(i, c, l) NGin::List<c*>::Iterator i = c::l.Begin(); for(;c::l.Count() > 0;++i)
#define nextc(i, c, l) if(i == c::l.End()) break;

#define foreachf(i, c, l) NGin::List<c*>::Iterator i = l.Begin(); for(;l.Count() > 0;++i)
#define nextf(i, c, l) if(i == l.End()) break;

#define foreachv(i, c, l) NGin::List<c>::Iterator i = l.Begin(); for(;l.Count() > 0;++i)
#define nextv(i, c, l) if(i == l.End()) break;

// Performs a ZeroMemory on the current class
#define ClearMemory(ClassName) char* t = (char*)this;for(int ii = 0; ii < sizeof(ClassName); ++ii){t[ii] = 0;};

// ClassDecls and ClassDef macros for setting up a class that automatically registers and removes itself on a list
// When using 'delete' on a class, make sure that you call ClassName::Clean() when you exit your current foreach loop
// so that the main list and be cleaned up.
#define ClassDecl(ClassName, ClassList, ClassDelete) ClassName();~ClassName();static NGin::List<ClassName*> ClassList; static NGin::List<ClassName*> ClassDelete; static void Delete(ClassName* Item); static void Clean();
#define ClassDef(ClassName, ClassList, ClassDelete) ClassName::ClassName() { ClearMemory(ClassName);ClassList.Add(this); }ClassName::~ClassName() { ClassList.Remove(this); }NGin::List<ClassName*> ClassName::ClassList;NGin::List<ClassName*> ClassName::ClassDelete;void ClassName::Delete(ClassName *Item){	if(!ClassDelete.Find(Item)){ClassDelete.Add(Item);}}void ClassName::Clean(){	foreachc(dLIt, ClassName, ClassDelete)	{		/*ClassName::ClassList.Remove(*dLIt);*/ delete *dLIt;		nextc(dLIt, ClassName, ClassDelete)	}	ClassName::ClassDelete.Clear();}
#define ClassDefInit(ClassName, ClassList, ClassDelete, ClassInit) ClassName::ClassName() { ClearMemory(ClassName);ClassList.Add(this); ClassInit();}ClassName::~ClassName() { ClassList.Remove(this); }List<ClassName*> ClassName::ClassList;List<ClassName*> ClassName::ClassDelete;void ClassName::Delete(ClassName *Item){	if(!ClassDelete.Find(Item)){ClassDelete.Add(Item);}}void ClassName::Clean(){	foreachc(dLIt, ClassName, ClassDelete)	{		/*ClassName::ClassList.Remove(*dLIt);*/ delete *dLIt;		nextc(dLIt, ClassName, ClassDelete)	}	ClassName::ClassDelete.Clear();}

#define ClassDefDB(ClassName, ClassList, ClassDelete) ClassName::ClassName() { ClearMemory(ClassName);ClassList.Add(this); }ClassName::~ClassName() { if(!ClassList.Remove(this)){OutputDebugString("Object instance '##ClassName##' not freed!");} }List<ClassName*> ClassName::ClassList;List<ClassName*> ClassName::ClassDelete;void ClassName::Delete(ClassName *Item){	if(!ClassDelete.Find(Item)){ClassDelete.Add(Item);}}void ClassName::Clean(){	foreachc(dLIt, ClassName, ClassDelete)	{		/*ClassName::ClassList.Remove(*dLIt);*/ delete *dLIt;		nextc(dLIt, ClassName, ClassDelete)	}	ClassName::ClassDelete.Clear();}


