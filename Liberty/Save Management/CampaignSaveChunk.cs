/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 2 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.Reach
{
    /// <summary>
    /// Describes chunk data in an mmiof.bmf file.
    /// </summary>
    /// <seealso cref="Process"/>
    internal class Chunk
    {
        /// <summary>
        /// Constructs a new Chunk object, reading header data out of a SaveReader.
        /// </summary>
        /// <param name="reader">
        /// The SaveReader to read from.
        /// On return, it will be positioned at the end of the entry list.
        /// </param>
        public Chunk(Liberty.SaveIO.SaveReader reader, ChunkOffset offset)
        {
            reader.Seek((long)offset, SeekOrigin.Begin);

            // Read header data
            _name = reader.ReadAscii(32);
            _entrySize = reader.ReadUInt32();
            reader.Seek(4, SeekOrigin.Current);
            _entryCount = reader.ReadUInt32();
            reader.Seek(36, SeekOrigin.Current);
            int entryListSize = reader.ReadInt32() - 0x54;
            _entryListStart = reader.BaseStream.Position;

            // Read the whole entry list into memory
            MemoryStream memoryStream = new MemoryStream(entryListSize);
            memoryStream.SetLength(entryListSize);
            reader.BaseStream.Read(memoryStream.GetBuffer(), 0, (int)entryListSize);
            _entryListStream = new Liberty.SaveIO.SaveReader(memoryStream);
        }

        /// <summary>
        /// Closes any streams opened by this chunk.
        /// </summary>
        public void Close()
        {
            _entryListStream.Close();
        }

        /// <summary>
        /// The name of the chunk.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// A SaveReader stream that can be used to read the entry list.
        /// </summary>
        public Liberty.SaveIO.SaveReader EntryReader
        {
            get { return _entryListStream; }
        }

        /// <summary>
        /// The position of the start of the entry list in the file.
        /// </summary>
        public long EntryListStart
        {
            get { return _entryListStart; }
        }

        /// <summary>
        /// The number of entries.
        /// </summary>
        public uint EntryCount
        {
            get { return _entryCount; }
        }

        /// <summary>
        /// The size of each entry.
        /// </summary>
        public uint EntrySize
        {
            get { return _entrySize; }
        }

        private string _name;
        private Liberty.SaveIO.SaveReader _entryListStream;
        private long _entryListStart;
        private uint _entryCount;
        private uint _entrySize;
    }

    #region "chunk offset list"
    /// <summary>
    /// The file offsets for each possible chunk.
    /// Chunks with the same name are suffixed with a number here, e.g. Contrail1.
    /// </summary>
    internal enum ChunkOffset
    {
        MegaloObjects = 0x2debc,
        SimObjectGlue = 0x368c0,
        WaterPhysicsCache = 0x3eb64,
        RecyclingGroup = 0x42e54,
        ObjectActivationRegions = 0x4bcec,
        DetHsThread = 0xa0c40,
        NonDetHsThread = 0x1083bc,
        HsGlobals = 0x1098c4,
        HsDistGlobals = 0x111b18,
        ImpactArrarys = 0x1174c0,
        Ragdolls = 0x1186a4,
        HavokProxies = 0x128ec4,
        CharacterPropertiesCache = 0x12935c,
        Actor = 0x139358,
        Swarm = 0x187888,
        Prop = 0x1881bc,
        PropRef = 0x193230,
        Tracking = 0x1a3304,
        PropSearch = 0x1a8f98,
        StimulusData = 0x1b1470,
        StimulusRef = 0x1b68e4,
        PathInfluence = 0x1be9b8,
        Squad = 0x1c3880,
        SquadGroup = 0x1d24f4,
        AiCue = 0x1d37a4,
        Objectives = 0x1d4418,
        Clump = 0x203874,
        JointState = 0x204dbc,
        DynamicFiringPoints = 0x205b84,
        AiDirectives = 0x20ae98,
        SquadPatrol = 0x20b9b0,
        Formations = 0x212648,
        Performance = 0x214fe0,
        PerformanceRuntimeDefinition = 0x218138,
        AiSyncActionArranger = 0x21f990,
        CommandScripts = 0x223468,
        VocalizationRecords = 0x22915c,
        Flocks = 0x229754,
        Boids = 0x22a184,
        LoopingSoundsRestoreState = 0x250000,
        ObjectLoopingSounds = 0x2523ec,
        CachedObjectRenderStates = 0x262000,
        ParticleSystem = 0x2b4d74,
        Particles = 0x2c0b10,
        ParticleEmitter = 0x306c64,
        ParticleLocation = 0x324e20,
        CParticleEmitterGpuSGames1 = 0x3326bc,
        CParticleEmitterGpuSGames2 = 0x338278,
        ContrailSystem = 0x3c6680,
        Contrail1 = 0x3c8768,
        Contrail2 = 0x3cc3d0,
        ContrailProfile = 0x3cf3b8,
        CContrailGpu = 0x3cf45c,
        CContrailGpuSRow = 0x3cfec4,
        LightVolumeSystem = 0x3f8c00,
        LightVolume1 = 0x3fa7e8,
        LightVolume2 = 0x3fc3d0,
        CLightVolumeGpu = 0x3fe4b8,
        CLightVolumeGpuSRow = 0x3fef20,
        BeamSystem = 0x413780,
        Beam1 = 0x415368,
        Beam2 = 0x4171d0,
        CBeamGpu = 0x4192b8,
        CBeamGpuSRow = 0x419d20,
        Players = 0x67f074,
        CollisionHierarchyNodeHeader = 0x68ec14,
        CollisionHierarchyElementHea = 0x6b8f68,
        Widget = 0x72bb80,
        Antenna = 0x72bedc,
        Cloth = 0x72c3e4,
        LeafSystem = 0x738c00,
        DeviceGroups = 0x73ff78,
        Lights = 0x74404c,
        Object = 0x7602f8,
        FireTeam = 0x914768,
        Lasing = 0x915500,
        BreakableSurfaceSetBrokenEv = 0x93a5d4,
        Effect = 0x95c68c,
        EffectEvent = 0x983fd4,
        EffectLocation = 0x98cdac,
        EffectGeometrySample = 0x9a6860,
        CheapParticleEmitters = 0x9b351c,
        ScreenEffect = 0x9b57ec,
        ShieldRenderCacheMessage = 0x9b6348,
        ChudWidgets1 = 0x9db440,
        ChudWidgets2 = 0x9dc4a4,
        ChudWidgets3 = 0x9dd508,
        ChudWidgets4 = 0x9de56c,
        ObjectListHeader = 0xa35ed4,
        ListObjectReference = 0xa36170,
        RecordedAnimations = 0xa367d4,
        Impacts = 0xa368e44
    }
    #endregion
}
