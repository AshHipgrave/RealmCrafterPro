#include <ITerrain.h>

namespace RealmCrafter
{
	namespace RCT
	{

		void Transform_PaintHole(ITerrain* Terrain, float Radius, NGin::Math::Vector2 Position, bool Circular, bool Erase)
		{
			Position -= NGin::Math::Vector2(Terrain->GetPosition().X, Terrain->GetPosition().Z);

			if(Circular)
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				int LoopRad = (int)Radius;

				// Square radius and get 'inner' radius
				Radius *= Radius;

				for(int ty = ((int)cy) - LoopRad - 1; ty < ((int)cy) + LoopRad + 1; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad - 1; tx < ((int)cx) + LoopRad + 1; ++tx)
					{
						// Get floating vertex positions
						float fx = (float)tx;
						float fy = (float)ty;

						// Get distance from brush center
						float Dist = pow(cx - fx, 2) + pow(cy - fy, 2);
										
						// Default height
						float H = 0.0f;

						// It's inside the circle
						if(Dist < Radius)
						{
							// Commit
							Terrain->SetExclusion(tx, ty, Erase);
						}
					}
				}
			}
			else
			{
				// Copy position
				float cx = Position.X;
				float cy = Position.Y;
				int LoopRad = (int)Radius;

	

				for(int ty = ((int)cy) - LoopRad; ty < ((int)cy) + LoopRad; ++ty)
				{
					for(int tx = ((int)cx) - LoopRad; tx < ((int)cx) + LoopRad; ++tx)
					{
						Terrain->SetExclusion(tx, ty, Erase);
					}
				}
			}
		}
	}
}
