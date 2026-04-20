using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace alligators_finalproject;

public class TextureAtlas
{
    private Dictionary<string, TextureRegion> _regions;
    private Dictionary<string, Animation> _animations;
    public Texture2D Texture { get; set; }

    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public void AddRegion(string key, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(key, region);
    }

    public void AddAnimation(string key, Animation animation)
    {
        _animations.Add(key, animation);
    }

    public TextureRegion GetRegion(string key)
    {
        return _regions[key];
    }

    public Animation GetAnimation(string key)
    {
        return _animations[key];
    }

    public void RemoveRegion(string key)
    {
        _regions.Remove(key);
    }

    public void RemoveAnimation(string key)
    {
        _animations.Remove(key);
    }

    public void Clear()
    {
        _regions.Clear();
        _animations.Clear();
    }

    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();
        string filePath = Path.Combine(content.RootDirectory, fileName);

        XmlReaderSettings settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Ignore
        };

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                var regions = root.Element("Regions")?.Elements("Region");

                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        string n = region.Attribute("n")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("w")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("h")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(n))
                        {
                            atlas.AddRegion(n, x, y, width, height);
                        }
                    }
                }

                var animations = root.Element("Animations")?.Elements("Animation");

                if (animations != null)
                {
                    foreach (var animationElement in animations)
                    {
                        string n = animationElement.Attribute("n")?.Value;
                        float time = float.Parse(animationElement.Attribute("time")?.Value ?? "0");

                        List<TextureRegion> frames = new List<TextureRegion>();
                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach (XElement frame in frameElements)
                            {
                                string regionName = frame.Attribute("region")?.Value;
                                TextureRegion region = atlas.GetRegion(regionName);
                                frames.Add(region);
                            }
                        }

                        Animation animation = new Animation(frames, time);
                        atlas.AddAnimation(n, animation);
                    }
                }
            }
        }

        return atlas;
    }

    public AnimatedSprite CreateSprite(string animationKey)
    {
        Animation animation = GetAnimation(animationKey);
        return new AnimatedSprite(animation);
    }

}