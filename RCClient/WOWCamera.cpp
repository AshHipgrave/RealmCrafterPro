#include "WOWCamera.h"
#include <quaternion.h>

// Implements a combined Third/First Person Camera
WOWCamera::WOWCamera()
{
	Pitch = 90.0f;
	Yaw = 0.0f;
	Zoom = 20.0f;

	YawSpeed = 0.0f;
	PitchSpeed = 0.0f;
	ZoomSpeed = 0.0f;

	InternalYaw = 0.0f;

	FCamX = FCamY = FCamZ = 0.0f;

	Player = 0;
	Head = 0;
	LastEN = 0;

	HeadX = HeadY = HeadZ = 0.0f;
	CameraDestX = CameraDestY = CameraDestZ = 0.0f;
	CameraDestPitch = CameraDestYaw = 0.0f;

	Moved = false;

	InvertAxis1 = -1;
	InvertAxis3 = -1;
}

bool WOWCamera::MovedThisFrame()
{
	return Moved;
}

void WOWCamera::SetActorInstance(ActorInstance* player)
{
	Player = player;
	Head = 0;
}

ActorInstance* WOWCamera::GetActorInstance()
{
	return Player;
}

uint sS = 0;

void WOWCamera::Update(uint camera)
{
	if(Player == 0)
		return;
	if(Player->EN == 0)
		return;
	//if(Player->EN != LastEN)
	//{
	//	Head = 0;
	//	LastEN = Player->EN;
	//}

	//if(Head == 0)
	//{
		Head = FindChild(Player->EN, "Head");
		if(Head == 0)
		{
			DebugLog("Head joint missing!");
			return;
		}
	//}

		HeadX = EntityX(Head, true);// - EntityX(Player->EN, true);
		HeadY = EntityY(Head, true);// - EntityY(Player->EN, true);
		HeadZ = EntityZ(Head, true);// - EntityZ(Player->EN, true);
	//}

	// Third person, essentially
	if(Zoom > 0.001f)
	{
		// Player is visible
		ShowEntity(Player->EN);
		
		// Find the head location
		float CenterX = HeadX;// + EntityX(Player->EN, true);
		float CenterY = HeadY;// + EntityY(Player->EN, true);
		float CenterZ = HeadZ;// + EntityZ(Player->EN, true);

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
		EntityAlpha(Player->EN, Alpha);

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
		if(RealmCrafter::Globals->LastZoneLoad < 3)
		{
			LerpX = FCamX;//CameraDestX;
			LerpY = FCamY;//CameraDestY;
			LerpZ = FCamZ;//CameraDestZ;
		}

		EntityPickMode(Player->CollisionEN, 0);
		uint Result = LinePick(CenterX, CenterY, CenterZ, LerpX - CenterX, LerpY - CenterY, LerpZ - CenterZ, 0.0f);
		EntityPickMode(Player->CollisionEN, 1);
 
		FCamX = LerpX;
		FCamY = LerpY;
		FCamZ = LerpZ;

		if(Result != 0 && RealmCrafter::Globals->LastZoneLoad >= 3)
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
				EntityAlpha(Player->EN, Alpha);

				//LerpX = PickPos.X;
				//LerpY = PickPos.Y;
				//LerpZ = PickPos.Z;

				LerpX = SetPos.X;
				LerpY = SetPos.Y;
				LerpZ = SetPos.Z;
			}
			
		}

		RotateEntity(camera, Lerp(CurrentPitch, CameraDestPitch, 0.3f), Lerp(LerpYaw, CameraDestYaw, 0.3f), 0.0f);
		PositionEntity(camera, LerpX, LerpY, LerpZ);

	}else // First person
	{
		// Can't see player
		HideEntity(Player->EN, false);

		// Find head location
		float CenterX = HeadX;// + EntityX(Player->EN, true);
		float CenterY = HeadY;// + EntityY(Player->EN, true);
		float CenterZ = HeadZ;// + EntityZ(Player->EN, true);

		// Set Camera
		RotateEntity(camera, -(Pitch - 90.0f), -Yaw, 0);
		PositionEntity(camera, CenterX, CenterY, CenterZ);
	}

	bool LeftDown = MouseDown(1);
	bool RightDown = MouseDown(2);

	ZoomSpeed += ((float)-MouseZSpeed()) * 0.02f;
	ZoomSpeed += JoyV() * 2.3f;
	ZoomSpeed -= JoyU() * 2.3f;

	if(LeftDown || RightDown)
	{
		if(!BlitzPlus::NGUIUpdateParameters.MouseBusy)
		{
			YawSpeed += MouseXSpeed();
			PitchSpeed += -MouseYSpeed() * ((Zoom > 0.001f) ? -InvertAxis3 : -InvertAxis1);
			InternalYaw += YawSpeed;

			if(RightDown || Zoom < 0.001f)
			{
				Player->Yaw += YawSpeed;
				
			}
		}
	}

	if(!NGin::Math::Vector3::FloatEqual(JoyZ(), 0.0f) || !NGin::Math::Vector3::FloatEqual(JoyPitch(), 0.0f))
	{
		float YawChange = JoyZ() * 1.3f;
		YawSpeed += YawChange;
		Player->Yaw += YawChange;
		InternalYaw += YawChange;

		PitchSpeed += (JoyPitch() * 0.01f) * ((Zoom > 0.001f) ? -InvertAxis3 : -InvertAxis1);
	}

	bool MoveOk = ChatEntry->Visible == false && TextInput::TextInputList.Count() == 0 && InDialog == false && SMemorising == 0;
	Moved = false;

	if((ControlDown(ControlLayout->Get("Forward")->ControlID) || JoyY() > 0.1f || (LeftDown && RightDown)) && MoveOk)
	{
		Player->Yaw = InternalYaw;
		Player->DestX = (Sin(Player->Yaw) * 7.0f) + Player->X;
		Player->DestZ = (Cos(Player->Yaw) * 7.0f) + Player->Z;

		Player->IsRunning = RealmCrafter::Globals->AlwaysRun;
		if(ControlDown(ControlLayout->Get("Run")->ControlID))
			Player->IsRunning = !Player->IsRunning;

		if(Player->Mount != 0)
			Player->Mount->IsRunning = Player->IsRunning;

		if(RealmCrafter::Globals->EnergyStat > -1 && Player->Mount == 0)
			if(Player->Attributes->Value[RealmCrafter::Globals->EnergyStat] <= 0)
				Player->IsRunning = false;
				
		RealmCrafter::Globals->AttackTarget = false;

		if(Player->WalkingBackward == true)
		{
			if(Player->Mount == 0)
			{
				if(CurrentSeq(Player) == Anim_Walk)
					Animate(Player->EN, 0);
			}else
			{
				if(CurrentSeq(Player->Mount) == Anim_Walk)
					Animate(Player->Mount->EN, 0);
			}
		}

		Moved = true;
		SetDestination(Player, Player->DestX, Player->DestZ, EntityY(Player->CollisionEN, true), false);
	}

	if((ControlDown(ControlLayout->Get("Backward")->ControlID) || JoyY() < -0.1f) && MoveOk)
	{
		Player->Yaw = InternalYaw;
		Player->DestX = (Sin(Player->Yaw + 180.0f) * 7.0f) + Player->X;
		Player->DestZ = (Cos(Player->Yaw + 180.0f) * 7.0f) + Player->Z;

		Player->IsRunning = false;
		RealmCrafter::Globals->AttackTarget = false;
		Player->WalkingBackward = true;
		if(Player->Mount != 0)
			Player->Mount->WalkingBackward = true;

		if(Player->WalkingBackward == false)
		{
			if(Player->Mount == 0)
			{
				if(CurrentSeq(Player) == Anim_Walk)
					Animate(Player->EN, 0);
			}else
			{
				if(CurrentSeq(Player->Mount) == Anim_Walk)
					Animate(Player->Mount->EN, 0);
			}
		}

		Moved = true;
		SetDestination(Player, Player->DestX, Player->DestZ, EntityY(Player->CollisionEN, true), false);
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

	RotateEntity(Player->CollisionEN, 0, -Player->Yaw + 180.0f, 0);

		// Underwater camera effects
	bool WasUnderwater = CameraUnderwater;
	CameraUnderwater = false;
	foreach(WIt, Water, WaterList)
	{
		Water* W = *WIt;

		if(EntityY(Cam) < EntityY(W->EN))
			if(Abs(EntityX(Cam) - EntityX(W->EN)) < (W->ScaleX / 2.0f))
				if(Abs(EntityZ(Cam) - EntityZ(W->EN)) < (W->ScaleZ / 2.0f))
				{
					CameraClsColor(Cam, W->Red, W->Green, W->Blue);
					CameraFogColor(Cam, W->Red, W->Green, W->Blue);
					FogNearNow = 1.0f;
					FogFarNow = 50.0f;
					Environment->SetViewDistance(FogNearNow, FogFarNow, false);
					CameraUnderwater = true;
					break;
				}
	
		next(WIt, Water, WaterList);
	}
	if(WasUnderwater == true)
		if(CameraUnderwater == false)
		{
			Environment->SetWeather(Environment->GetWeather());
		}
}

inline float WOWCamera::Lerp(float a, float b, float t)
{
	return a + ((b - a) * t);
}
