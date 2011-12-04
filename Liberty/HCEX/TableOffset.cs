using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.HCEX
{
    public enum TableOffset : long
    {
        CachedObjectRenderStates = 0x5788,
        Widget = 0x157C0,
        Flag = 0x15AF8,
        Antenna = 0x188A8,
        Glow = 0x1A9B0,
        GlowParticles = 0x1BCC8,
        LightVolumes = 0x28500,
        Lightnings = 0x28D38,
        DeviceGroups = 0x29578,
        Lights = 0x2B5B0,
        ClusterLightReference = 0x46FEC,
        LightClusterReference = 0x4D024,
        Object = 0x5305C,
        ClusterCollideableObjectRe = 0x25A164,
        CollideableObjectClusterRe = 0x26019C,
        ClusterNoncollideableObject = 0x2669D4,
        NoncollideableObjectCluster = 0x26CA0C,
        Decals = 0x28107C,
        DecalVertexCache = 0x29F904,
        Players = 0x2AD93C,
        Teams = 0x2B1974,
        Contrail = 0x2B22DC,
        ContrailPoint = 0x2B6714,
        Particle = 0x2C474C,
        Effect = 0x2E0784,
        EffectLocation = 0x2F03BC,
        ParticleSystems = 0x2FC6F4,
        ParticleSystemParticles = 0x301D2C,
        ObjectLoopingSounds = 0x311FC8,
        Actor = 0x31FF18,
        Swarm = 0x392350,
        SwarmComponent = 0x393688,
        Prop = 0x3976C0,
        Encounter = 0x3D1EF8,
        AiPursuit = 0x3DE530,
        AiConversation = 0x3E16B8,
        ObjectListHeader = 0x3E1A10,
        ListObjectReference = 0x3E1C88,
        HsThread = 0x3E22C0,
        HsGlobals = 0x403AF8,
        RecordedAnimations = 0x405B30
    }
}
