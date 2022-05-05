using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile.../Animated Tile", fileName = "New Animated Tile")]
public class AnimatedTile : TileBase
{
	public Sprite[] sprites;
	public float minSpeed = 1;
	public float maxSpeed = 1;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		if(sprites != null && sprites.Length > 0)
		{
			tileData.sprite = sprites[0];
		}
	}

	public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
	{
		if(sprites.Length > 0)
		{
			tileAnimationData.animatedSprites = sprites;
			tileAnimationData.animationSpeed = Random.Range(minSpeed, maxSpeed);
			tileAnimationData.animationStartTime = 0;

			return true;
		}

		return false;
	}
}
