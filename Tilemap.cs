using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class Tilemap
{
    // haven't fully implemented collision, but the sand blocks should be impassable   
    // https://docs.monogame.net/articles/tutorials/building_2d_games/12_collision_detection/index.html
    // bounds should also be implemented for enemies, player, and the coins
    private readonly Tileset _tileset;
    private readonly int[] _tiles;
    
    public int Rows { get;  }
    public int Columns { get; }
    public int Count { get; }
    public Vector2 Scale { get; set; }
    public float TileWidth => _tileset.TileWidth * Scale.X;
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = new Vector2(1.0f);
        _tiles = new int[Count];
    }

    public void SetTile(int index, int tileID)
    {
        _tiles[index] = tileID;
    }

    public void SetTile(int column, int row, int tileID)
    {
        int index = row * Columns + column;
        SetTile(index, tileID);
    }

    public TextureRegion GetTile(int index)
    {
        return _tileset.GetTile(_tiles[index]);
    }

    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int tileID = _tiles[i];
            TextureRegion tile = _tileset.GetTile(tileID);

            int x = i % Columns;
            int y = i / Columns;
            
            Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
            tile.Draw(spriteBatch, position, 0, Vector2.Zero, 
                Scale, SpriteEffects.None, 1);
        }
    }

    public static Tilemap FromFile(ContentManager content, string fileName)
    {
        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                XElement tilesetElement = root.Element("Tileset");
                
                string regionAttribute = tilesetElement.Attribute("region").Value;
                string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);
                int width = int.Parse(split[2]);
                int height = int.Parse(split[3]);

                int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                string path = tilesetElement.Value;
                
                Texture2D texture = content.Load<Texture2D>(path);
                TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);
                Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                XElement tilesElement = root.Element("Tiles");
                string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
                Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                for (int row = 0; row < rows.Length; row++)
                {
                    string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    for (int column = 0; column < columnCount; column++)
                    {
                        int tilesetIndex = int.Parse(columns[column]);
                        tilemap.SetTile(column, row, tilesetIndex);
                    }
                }
                return tilemap;
            }
        }
      
    }
}