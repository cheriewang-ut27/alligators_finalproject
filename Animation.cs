using System;
using System.Collections.Generic;

namespace alligators_finalproject;

public class Animation
{
    public List<TextureRegion> Frames { get; set; }
    public float FrameTime { get; set; }
    public int FrameCount { get; set; }

    public Animation()
    {
        Frames = new List<TextureRegion>();
        FrameTime = 1.0f / 10.0f;
    }
    
    public Animation(List<TextureRegion> frames,  float frameTime)
    {
        Frames = frames;
        FrameTime = frameTime;
    }
}