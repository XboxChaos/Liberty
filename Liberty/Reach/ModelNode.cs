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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Reach
{
    /// <summary>
    /// A node in an object's model data.
    /// </summary>
    public class ModelNode
    {
        public float Scale;
        public MathUtil.Vector3 Right;
        public MathUtil.Vector3 Forward;
        public MathUtil.Vector3 Up;
        public float X;
        public float Y;
        public float Z;

        public static ModelNode FromSaveReader(SaveIO.SaveReader reader)
        {
            ModelNode part = new ModelNode();

            part.Scale = reader.ReadFloat();
            part.Right.X = reader.ReadFloat();
            part.Right.Y = -reader.ReadFloat();
            part.Right.Z = reader.ReadFloat();
            part.Forward.X = -reader.ReadFloat();
            part.Forward.Y = reader.ReadFloat();
            part.Forward.Z = -reader.ReadFloat();
            part.Up.X = reader.ReadFloat();
            part.Up.Y = -reader.ReadFloat();
            part.Up.Z = reader.ReadFloat();
            part.X = reader.ReadFloat();
            part.Y = reader.ReadFloat();
            part.Z = reader.ReadFloat();

            return part;
        }

        public void Update(SaveIO.SaveWriter writer, GameObject parent)
        {
            if (_relative)
            {
                // Find the parent object's position in world space
                float parentX = parent.X, parentY = parent.Y, parentZ = parent.Z;
                Reach.GameObject obj = parent.Carrier;
                while (obj != null)
                {
                    parentX += obj.X;
                    parentY += obj.Y;
                    parentZ += obj.Z;
                    obj = obj.Carrier;
                }

                // Transform the coords to world space and write them
                writer.WriteFloat(Scale * parent.Scale);
                writer.WriteFloat(Right.X);
                writer.WriteFloat(-Right.Y);
                writer.WriteFloat(Right.Z);
                writer.WriteFloat(-Forward.X);
                writer.WriteFloat(Forward.Y);
                writer.WriteFloat(-Forward.Z);
                writer.WriteFloat(Up.X);
                writer.WriteFloat(-Up.Y);
                writer.WriteFloat(Up.Z);
                writer.WriteFloat(X + parentX);
                writer.WriteFloat(Y + parentY);
                writer.WriteFloat(Z + parentZ);
            }
            else
            {
                // The coords are already absolute, just write them
                writer.WriteFloat(Scale);
                writer.WriteFloat(Right.X);
                writer.WriteFloat(-Right.Y);
                writer.WriteFloat(Right.Z);
                writer.WriteFloat(-Forward.X);
                writer.WriteFloat(Forward.Y);
                writer.WriteFloat(-Forward.Z);
                writer.WriteFloat(Up.X);
                writer.WriteFloat(-Up.Y);
                writer.WriteFloat(Up.Z);
                writer.WriteFloat(X);
                writer.WriteFloat(Y);
                writer.WriteFloat(Z);
            }
        }

        /// <summary>
        /// Converts this node to use coordinates relative to a parent object.
        /// By default, node coordinates are in world space.
        /// </summary>
        /// <param name="parent">The node's owner object.</param>
        public void MakeRelative(GameObject parent)
        {
            if (_relative)
                return;

            Scale /= parent.Scale;

            // The X, Y, and Z coords are absolute positions by default
            // So we have to scan through the entire carry hierarchy to get a relative position
            GameObject obj = parent;
            while (obj != null)
            {
                X -= obj.X;
                Y -= obj.Y;
                Z -= obj.Z;
                obj = obj.Carrier;
            }

            _relative = true;
        }

        public const ushort DataSize = 13 * 4;

        private bool _relative = false;
    }

    /// <summary>
    /// A read-only collection of ModelNodes.
    /// </summary>
    public class NodeCollection : ReadOnlyCollectionBase
    {
        public NodeCollection(IList<ModelNode> nodes)
        {
            InnerList.AddRange((IList)nodes);
        }

        public ModelNode this[int index]
        {
            get
            {
                return (ModelNode)InnerList[index];
            }
        }

        public int IndexOf(ModelNode node)
        {
            return InnerList.IndexOf(node);
        }

        public bool Contains(ModelNode node)
        {
            return InnerList.Contains(node);
        }

        public void MakeRelative(GameObject parent)
        {
            foreach (ModelNode node in InnerList)
                node.MakeRelative(parent);
        }
    }
}
