//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#include "CCameraController.h"
#include <quaternion.h>

namespace RealmCrafter
{

	// Implements a combined Third/First Person Camera
	CCameraController::CCameraController()
		: Layout(NULL)
	{
		Pitch = 90.0f;
		Yaw = 0.0f;
		Zoom = 20.0f;

		YawSpeed = 0.0f;
		PitchSpeed = 0.0f;
		ZoomSpeed = 0.0f;

		InternalYaw = 0.0f;

		FCamX = FCamY = FCamZ = 0.0f;

		Head = 0;
		LastEN = 0;

		HeadX = HeadY = HeadZ = 0.0f;
		CameraDestX = CameraDestY = CameraDestZ = 0.0f;
		CameraDestPitch = CameraDestYaw = 0.0f;

		MouseMove = false;
		Moved = false;

		InvertAxis1 = -1;
		InvertAxis3 = -1;

		LastSectorX = 0;
		LastSectorZ = 0;
		SectorMoveX = 0;
		SectorMoveZ = 0;

		DELSCount = 0;
	}

	CCameraController::~CCameraController()
	{
	}

	void CCameraController::SetControlLayout(IControlLayout* layout)
	{
		Layout = layout;
		Layout->AddInstance("Movement", "Forward", 17);
		Layout->AddInstance("Movement", "Backward", 31);
		Layout->AddInstance("Movement", "Run", 42);
		Layout->AddInstance("Movement", "Strafe Left", 16);
		Layout->AddInstance("Movement", "Strafe Right", 18);
		Layout->AddInstance("Movement", "Turn Left", 30);
		Layout->AddInstance("Movement", "Turn Right", 32);
	}

	void CCameraController::ResetCameraPosition(float x, float y, float z)
	{
		FCamX = x;
		FCamY = y;
		FCamZ = z;
	}

	bool CCameraController::MovedThisFrame()
	{
		return Moved;
	}

	uint sS = 0;

	void CCameraController::Update(uint camera, SDK::IActorInstance* player)
	{
		

		if(Layout == NULL)
		{
			return;
		}else
		{
			if(Layout->Get("Forward") == NULL)
				return;
			if(Layout->Get("Run") == NULL)
				return;
			if(Layout->Get("Backward") == NULL)
				return;
			if(Layout->Get("Strafe Left") == NULL)
				return;
			if(Layout->Get("Strafe Right") == NULL)
				return;
			if(Layout->Get("Turn Left") == NULL)
				return;
			if(Layout->Get("Turn Right") == NULL)
				return;
		}

		if(player == 0)
			return;
		if(player->GetEN() == 0)
			return;

		Head = FindChild(player->GetEN(), "Head");
		if(Head == 0)
			return;

		HeadX = EntityX(Head, true);
		HeadY = EntityY(Head, true);
		HeadZ = EntityZ(Head, true);

		// Check if we moved sector, if so, move camera
		int CurrentSectorX = player->GetPosition().SectorX;
		int CurrentSectorZ = player->GetPosition().SectorZ;
		bool SC = false;

		if(CurrentSectorX != LastSectorX
			|| CurrentSectorZ != LastSectorZ)
		{
			DELSCount = 0;


			SectorMoveX = ((CurrentSectorX - LastSectorX) * RealmCrafter::SectorVector::SectorSize);
			SectorMoveZ = ((CurrentSectorZ - LastSectorZ) * RealmCrafter::SectorVector::SectorSize);

			PositionEntity(camera,
				EntityX(camera) - SectorMoveX,
				EntityY(camera),
				EntityZ(camera) - SectorMoveZ);

			FCamX -= SectorMoveX;
			FCamZ -= SectorMoveZ;
		}

		if(DELSCount < 3)
		{
			// NOTE: The head bone is just a pivot animated as a child of the actor mesh.
			// Because of this, it isn't re-transformed when the player pivot is. Instead,
			// it is updated when the animation matrices are calculated for rendering in
			// order to increase efficiency. The down side is that our results are a frame
			// behind, so need to be transformed on sector change!
			HeadX -= SectorMoveX;
			HeadZ -= SectorMoveZ;
		}
		LastSectorX = CurrentSectorX;
		LastSectorZ = CurrentSectorZ;

		// Third person
		if(Zoom > 0.001f)
		{
			// Player is visible
			ShowEntity(player->GetEN());

			// Find the head location
			float CenterX = HeadX;
			float CenterY = HeadY;
			float CenterZ = HeadZ;

			// Find the Yaw Offset
			float YawX = Sin(Yaw) * -Zoom;
			float YawZ = Cos(Yaw) * -Zoom;

			// Find the Pitch Offset (Z is not multiplied so that it can be used later)
			float PitchZ = Sin(Pitch);
			float PitchY = Cos(Pitch) * Zoom;

			// Set player alpha
			float Alpha = (Zoom - 1.0f) * 0.5f;
			if(Alpha > 1.0f)
				Alpha = 1.0f;
			if(Alpha < 0.0f)
				Alpha = 0.0f;
			EntityAlpha(player->GetEN(), Alpha);

			// Set Camera
			CameraDestPitch = -(Pitch - 90);
			CameraDestYaw = -Yaw;
			CameraDestX = (YawX * PitchZ) + CenterX;
			CameraDestY = CenterY + PitchY;
			CameraDestZ = (YawZ * PitchZ) + CenterZ;

			float CurrentPitch = EntityPitch(camera, true);
			float CurrentYaw = EntityYaw(camera, true);
			float CurrentX = FCamX;//EntityX(camera, true);
			float CurrentY = FCamY;//EntityY(camera, true);
			float CurrentZ = FCamZ;//EntityZ(camera, true);

			if(DELSCount < 5)
			{
// 				printf("Current: %f, %f, %f\n", CurrentX, CurrentY, CurrentZ);
// 				printf("Dest: %f, %f, %f\n", CameraDestX, CameraDestY, CameraDestZ);

				++DELSCount;
			}

			while(CurrentYaw < 0.0f)
				CurrentYaw += 360.0f;
			while(CurrentYaw >= 360.0f)
				CurrentYaw -= 360.0f;

			while(CameraDestYaw < 0.0f)
				CameraDestYaw += 360.0f;
			while(CameraDestYaw >= 360.0f)
				CameraDestYaw -= 360.0f;

			while(CurrentPitch > 180.0f)
				CurrentPitch -= 360.0f;

			if(EntityRoll(camera, true) > 0.01f)
			{
				CurrentYaw = (90.0f - CurrentYaw) + 90.0f;
				while(CurrentYaw < 0.0f)
					CurrentYaw += 360.0f;
				while(CurrentYaw >= 360.0f)
					CurrentYaw -= 360.0f;

				CurrentPitch -= 180.0f;
			}

			if(CurrentPitch < -180.0f)
				CurrentPitch += 360.0f;



			float LerpYaw = CurrentYaw;
			if(LerpYaw < CameraDestYaw && CameraDestYaw - LerpYaw > 270.0f)
				LerpYaw += 360.0f;
			if(LerpYaw > CameraDestYaw && LerpYaw - CameraDestYaw > 270.0f)
				LerpYaw -= 360.0f;

			float LerpX = Lerp(CurrentX, CameraDestX, 0.3f);
			float LerpY = Lerp(CurrentY, CameraDestY, 0.3f);
			float LerpZ = Lerp(CurrentZ, CameraDestZ, 0.3f);

			// Reset if we loaded a zone (to fix the camera smoothly moving to player)
			if(Globals->LastZoneLoad < 3)
			{
				LerpX = FCamX;
				LerpY = FCamY;
				LerpZ = FCamZ;
			}

			EntityPickMode(player->GetCollisionEN(), 0);
			uint Result = LinePick(CenterX, CenterY, CenterZ, LerpX - CenterX, LerpY - CenterY, LerpZ - CenterZ, 0.0f);
			EntityPickMode(player->GetCollisionEN(), 1);

			FCamX = LerpX;
			FCamY = LerpY;
			FCamZ = LerpZ;

			if(Result != 0 && Globals->LastZoneLoad >= 3)
			{
				NGin::Math::Vector3 SetPos(LerpX, LerpY, LerpZ);
				NGin::Math::Vector3 PickPos(PickedX(), PickedY(), PickedZ());
				NGin::Math::Vector3 CenterPos(CenterX, CenterY, CenterZ);

				float SetDistance = (CenterPos - SetPos).Length();
				float PickDistance = (CenterPos - PickPos).Length();



				if(PickDistance < SetDistance)
				{
					SetPos = (SetPos - CenterPos);
					SetPos.Normalize();
					SetPos *= PickDistance - 1.5f;
					SetPos += CenterPos;

					Alpha = (PickDistance -1.0f) * 0.5f;
					if(Alpha > 1.0f)
						Alpha = 1.0f;
					if(Alpha < 0.0f)
						Alpha = 0.0f;
					EntityAlpha(player->GetEN(), Alpha);

					LerpX = SetPos.X;
					LerpY = SetPos.Y;
					LerpZ = SetPos.Z;
				}

			}

			RotateEntity(camera, Lerp(CurrentPitch, CameraDestPitch, 0.3f), Lerp(LerpYaw, CameraDestYaw, 0.3f), 0.0f);
			PositionEntity(camera, LerpX, LerpY, LerpZ);
			//PositionEntity(camera, CameraDestX, CameraDestY, CameraDestZ);

		}else // First person
		{
			// Can't see player
			HideEntity(player->GetEN());

			// Find head location
			float CenterX = HeadX;
			float CenterY = HeadY;
			float CenterZ = HeadZ;

			// Set Camera
			RotateEntity(camera, -(Pitch - 90.0f), -Yaw, 0);
			PositionEntity(camera, CenterX, CenterY, CenterZ);
		}

		bool LeftDown = ControlDown(501);
		bool RightDown = ControlDown(502);

		ZoomSpeed += ((float)-MouseZSpeed()) * 0.02f;


		// If right mouse isn't down (the one which turns the actor) then we
		// can check to turn with keys
		if(!RightDown && !LeftDown)
		{
			YawSpeed += (ControlDown(Layout->Get("Turn Left")->ControlID) ? (-2.0f * Globals->DeltaTime) : 0.0f);
			YawSpeed += (ControlDown(Layout->Get("Turn Right")->ControlID) ? (2.0f * Globals->DeltaTime) : 0.0f);
			InternalYaw += YawSpeed;

			player->SetYaw(player->GetYaw() + YawSpeed);
		}

		if(LeftDown || RightDown)
		{
			if(!SDK::NGUIUpdateParameters->MouseBusy || MouseMove == true)
			{
				POINT MouseTemp;
				GetCursorPos(&MouseTemp);
				//YawSpeed += ((float)MouseXSpeed() * Globals->DeltaTime);
				//PitchSpeed += -((float)MouseYSpeed() * Globals->DeltaTime) * ((Zoom > 0.001f) ? -InvertAxis3 : -InvertAxis1);

				if(RightDown || Zoom < 0.001f)
				{
					player->SetYaw(player->GetYaw() + YawSpeed);
				}

				if(MouseMove != true)
				{
					GetCursorPos(&MouseStore);
				}
				else
				{
					float MXSpeed = 0;
					float MYSpeed = 0;
					float SpeedCoef = 0.25;

					if(MouseTemp.x != MouseStore.x)
						MXSpeed = ((float)((MouseTemp.x - MouseStore.x)));

					if(MouseTemp.y != MouseStore.y)
						MYSpeed = ((float)((MouseTemp.y - MouseStore.y)));

					YawSpeed   += (MXSpeed * SpeedCoef * Globals->DeltaTime)  ;
					PitchSpeed += -(MYSpeed * SpeedCoef * Globals->DeltaTime) * ((Zoom > 0.001f) ? -InvertAxis3 : -InvertAxis1);

					InternalYaw += YawSpeed;
					SetCursorPos(MouseStore.x, MouseStore.y);
				}
				MouseMove = true;
			}
			else
			{
				MouseMove = false;
			}
		}
		else
		{
			MouseMove = false;
		}
		
		bool MoveOk = CanMovePlayer();
		Moved = false;

		bool StrafeLeft = (ControlDown(Layout->Get("Strafe Left")->ControlID));
		bool StrafeRight = (ControlDown(Layout->Get("Strafe Right")->ControlID));

		// Walking forward
		if((ControlDown(Layout->Get("Forward")->ControlID) || (LeftDown && RightDown)) && MoveOk)
		{
			player->SetYaw(InternalYaw);
			float DestX = (Sin(player->GetYaw()) * 7.0f) + player->GetPosition().X;
			float DestZ = (Cos(player->GetYaw()) * 7.0f) + player->GetPosition().Z;

			player->SetRunning(Globals->AlwaysRun);
			if(ControlDown(Layout->Get("Run")->ControlID))
				player->SetRunning(!player->GetRunning());

			if(player->GetMount() != 0)
				player->GetMount()->SetRunning(player->GetRunning());

			if(Globals->EnergyStat > -1 && player->GetMount() == 0)
				if(player->GetAttributes()->Value[Globals->EnergyStat] <= 0)
					player->SetRunning(false);

			player->SetStrafingLeft(false);
			player->SetStrafingRight(false);

			Globals->AttackTarget = false;

			Moved = true;

			RealmCrafter::SectorVector Destination = player->GetPosition();
			Destination.X = DestX;
			Destination.Z = DestZ;
			Destination.Y = EntityY(player->GetCollisionEN(), true);
			player->SetDestination(Destination, false);
		}

		// Walking backwards
		else if((ControlDown(Layout->Get("Backward")->ControlID)) && MoveOk)
		{
			player->SetYaw(InternalYaw);
			float DestX = (Sin(player->GetYaw()) * -7.0f) + player->GetPosition().X;
			float DestZ = (Cos(player->GetYaw()) * -7.0f) + player->GetPosition().Z;

			player->SetRunning(false);
			player->SetStrafingLeft(false);
			player->SetStrafingRight(false);
			Globals->AttackTarget = false;
			player->SetWalkingBackward(true);
			if(player->GetMount() != 0)
				player->GetMount()->SetWalkingBackward(true);


			Moved = true;

			RealmCrafter::SectorVector Destination = player->GetPosition();
			Destination.X = DestX;
			Destination.Z = DestZ;
			Destination.Y = EntityY(player->GetCollisionEN(), true);
			player->SetDestination(Destination, false);
		}

		// Strafe Left
		else if(((StrafeLeft || StrafeRight) && !(StrafeLeft && StrafeRight)) && MoveOk && player->GetMount() == NULL)
		{
			float Angle = 0.0f;
			if(StrafeLeft)
				Angle += 90.0f;
			if(StrafeRight)
				Angle -= 90.0f;

			player->SetYaw(InternalYaw);
			float DestX = (Sin(player->GetYaw() + Angle) * -7.0f) + player->GetPosition().X;
			float DestZ = (Cos(player->GetYaw() + Angle) * -7.0f) + player->GetPosition().Z;

			player->SetRunning(false);
			Globals->AttackTarget = false;

			// Temp: Make these strafe
			player->SetWalkingBackward(false);
			player->SetStrafingLeft(StrafeLeft);
			player->SetStrafingRight(StrafeRight);
			//player->SetWalkingBackward(true);
			//if(player->GetMount() != 0)
			//	player->GetMount()->SetWalkingBackward(true);


			Moved = true;

			RealmCrafter::SectorVector Destination = player->GetPosition();
			Destination.X = DestX;
			Destination.Z = DestZ;
			Destination.Y = EntityY(player->GetCollisionEN(), true);
			player->SetDestination(Destination, false);

		}

		// Nothing down, stop movement
		else
		{
			// Don't reset destination if its already done, this prevents the Quit Progress bar from being cancelled
			// Also, don't update this if the player is in combat (since he chases the target).
			if(player->GetDestination() != player->GetPosition() && !Globals->AttackTarget)
			{
				player->SetDestination(player->GetPosition(), false);
			}
		}

		Pitch += PitchSpeed;
		Yaw += YawSpeed;
		Zoom += ZoomSpeed;

		PitchSpeed *= 0.1f;
		YawSpeed *= 0.1f;
		ZoomSpeed *= 0.001f;

		if(Pitch > 178.0f)
			Pitch = 178.0f;
		if(Pitch < 2.0f)
			Pitch = 2.0f;

		if(Zoom < 0.0f)
			Zoom = 0.0f;
		if(Zoom > 20.0f)
			Zoom = 20.0f;

		RotateEntity(player->GetCollisionEN(), 0, -player->GetYaw() + 180.0f, 0);
	}

	inline float CCameraController::Lerp(float a, float b, float t)
	{
		return a + ((b - a) * t);
	}
}
