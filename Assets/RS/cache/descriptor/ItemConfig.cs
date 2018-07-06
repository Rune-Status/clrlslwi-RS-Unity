using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RS
{
    public class ItemParticleConfig
    {
        public int vertexIndex;
        public double startSize;
        public double r;
        public double g;
        public double b;
        public double a;
        public double gravityModifier;
        public double startLifetime;
        public double startSpeed;
        public double emissionRate;
    }

    /// <summary>
    /// A config that describes an in game item.
    /// </summary>
    public class ItemConfig
    {
        public string[] action;
        public int brightness;
        public string description;
        public int femaleDialogModel1;
        public int femaleDialogModel2;
        public int femaleModel1;
        public int femaleModel2;
        public int femaleModel3;
        public int femaleOffY;
        public string[] groundAction;
        public int iconDist;
        public int iconPitch;
        public int iconRoll;
        public int iconX;
        public int iconY;
        public int iconYaw;
        public int index = -1;
        public bool isMembers;
        public int maleDialogModel1;
        public int maleDialogModel2;
        public int maleModel1;
        public int maleModel2;
        public int maleModel3;
        public int maleOffY;
        public int modelIndex;
        public string name;
        public int[] newColor;
        public int noteItemIndex;
        public int noteTemplateIndex;
        public int[] oldColor;
        public int pilePriority;
        public int scaleX;
        public int scaleY;
        public int scaleZ;
        public int specular;
        public int[] stackAmount;
        public int[] stackIndex;
        public bool stackable;
        public int team;
        public ItemParticleConfig[] particleConfigs;

        public ItemConfig(JagexBuffer b)
        {
            SetDefaults();
            ParseFrom(b);
        }

        private void ParseFrom(JagexBuffer b)
        {
            int opcode = b.ReadUByte();
            while (opcode != 0)
            {
                ParseOpcode(opcode, b);
                opcode = b.ReadUByte();
            }
        }

        private void ParseOpcode(int opcode, JagexBuffer b)
        {
            if (opcode == 1)
            {
                modelIndex = (short)b.ReadUShort();
            }
            else if (opcode == 2)
            {
                name = b.ReadString(10);
            }
            else if (opcode == 3)
            {
                description = b.ReadString(10);
            }
            else if (opcode == 4)
            {
                iconDist = (short)b.ReadUShort();
            }
            else if (opcode == 5)
            {
                iconPitch = (short)b.ReadUShort();
            }
            else if (opcode == 6)
            {
                iconYaw = (short)b.ReadUShort();
            }
            else if (opcode == 7)
            {
                int x = b.ReadUShort();

                if (x > 32767)
                {
                    x -= 0x10000;
                }

                iconX = (short)x;
            }
            else if (opcode == 8)
            {
                int y = b.ReadUShort();

                if (y > 32767)
                {
                    y -= 0x10000;
                }

                iconY = (short)y;
            }
            else if (opcode == 10)
            {
                b.ReadUShort();
            }
            else if (opcode == 11)
            {
                stackable = true;
            }
            else if (opcode == 12)
            {
                pilePriority = b.ReadInt();
            }
            else if (opcode == 16)
            {
                isMembers = true;
            }
            else if (opcode == 23)
            {
                maleModel1 = (short)b.ReadUShort();
                maleOffY = b.ReadByte();
            }
            else if (opcode == 24)
            {
                maleModel2 = (short)b.ReadUShort();
            }
            else if (opcode == 25)
            {
                femaleModel1 = (short)b.ReadUShort();
                femaleOffY = b.ReadByte();
            }
            else if (opcode == 26)
            {
                femaleModel2 = (short)b.ReadUShort();
            }
            else if (opcode >= 30 && opcode < 35)
            {
                if (groundAction == null)
                {
                    groundAction = new string[5];
                }

                groundAction[opcode - 30] = b.ReadString(10);
            }
            else if (opcode >= 35 && opcode < 40)
            {
                if (action == null)
                {
                    action = new String[5];
                }

                var actionIdx = opcode - 35;
                action[actionIdx] = b.ReadString(10);
            }
            else if (opcode == 40)
            {
                int j = b.ReadUByte();
                oldColor = new int[j];
                newColor = new int[j];
                for (int k = 0; k < j; k++)
                {
                    oldColor[k] = b.ReadUShort();
                    newColor[k] = b.ReadUShort();
                }

            }
            else if (opcode == 78)
            {
                maleModel3 = (short)b.ReadUShort();
            }
            else if (opcode == 79)
            {
                femaleModel3 = (short)b.ReadUShort();
            }
            else if (opcode == 90)
            {
                maleDialogModel1 = (short)b.ReadUShort();
            }
            else if (opcode == 91)
            {
                femaleDialogModel1 = (short)b.ReadUShort();
            }
            else if (opcode == 92)
            {
                maleDialogModel2 = (short)b.ReadUShort();
            }
            else if (opcode == 93)
            {
                femaleDialogModel2 = (short)b.ReadUShort();
            }
            else if (opcode == 95)
            {
                iconRoll = (short)b.ReadUShort();
            }
            else if (opcode == 97)
            {
                noteItemIndex = (short)b.ReadUShort();
            }
            else if (opcode == 98)
            {
                noteTemplateIndex = (short)b.ReadUShort();
            }
            else if (opcode >= 100 && opcode < 110)
            {
                if (stackIndex == null)
                {
                    stackIndex = new int[10];
                    stackAmount = new int[10];
                }
                stackIndex[opcode - 100] = (short)b.ReadUShort();
                stackAmount[opcode - 100] = b.ReadUShort();
            }
            else if (opcode == 110)
            {
                scaleX = (short)b.ReadUShort();
            }
            else if (opcode == 111)
            {
                scaleY = (short)b.ReadUShort();
            }
            else if (opcode == 112)
            {
                scaleZ = (short)b.ReadUShort();
            }
            else if (opcode == 113)
            {
                brightness = b.ReadByte();
            }
            else if (opcode == 114)
            {
                specular = (short)(b.ReadByte() * 5);
            }
            else if (opcode == 115)
            {
                team = b.ReadByte();
            }
        }

        public void SetDefaults()
        {
            modelIndex = 0;
            name = null;
            description = null;
            oldColor = null;
            newColor = null;
            iconDist = 2000;
            iconPitch = 0;
            iconYaw = 0;
            iconRoll = 0;
            iconX = 0;
            iconY = 0;
            stackable = false;
            pilePriority = 1;
            isMembers = false;
            groundAction = null;
            action = null;
            maleModel1 = -1;
            maleModel2 = -1;
            maleOffY = 0;
            femaleModel1 = -1;
            femaleModel2 = -1;
            femaleOffY = 0;
            maleModel3 = -1;
            femaleModel3 = -1;
            maleDialogModel1 = -1;
            maleDialogModel2 = -1;
            femaleDialogModel1 = -1;
            femaleDialogModel2 = -1;
            stackIndex = null;
            stackAmount = null;
            noteItemIndex = -1;
            noteTemplateIndex = -1;
            scaleX = 128;
            scaleY = 128;
            scaleZ = 128;
            brightness = 0;
            specular = 0;
            team = 0;
        }

        public Model GetDialogModel(int gender)
        {
            int a = maleDialogModel1;
            int b = maleDialogModel2;

            if (gender == 1)
            {
                a = femaleDialogModel1;
                b = femaleDialogModel2;
            }

            if (a == -1)
            {
                return null;
            }

            Model mesh = new Model(a);

            if (b != -1)
            {
                mesh = new Model(2, new Model[] { mesh, new Model(b) });
            }

            if (oldColor != null)
            {
                mesh.SetColors(oldColor, newColor);
            }
            return mesh;
        }

        public Model GetModel(int count, bool applyLighting = true)
        {
            if (stackIndex != null && count > 1)
            {
                int index = -1;
                for (int i = 0; i < 10; i++)
                {
                    if (count >= stackAmount[i] && stackAmount[i] != 0)
                    {
                        index = stackIndex[i];
                    }
                }
                if (index != -1)
                {
                    return GameContext.Cache.GetItemConfig(index).GetModel(1, applyLighting);
                }
            }

            Model mesh = new Model(modelIndex);
            if (mesh == null)
            {
                return null;
            }

            if (scaleX != 128 || scaleY != 128 || scaleZ != 128)
            {
                mesh.Scale(scaleX, scaleY, scaleZ);
            }

            if (oldColor != null)
            {
                mesh.SetColors(oldColor, newColor);
            }

            if (applyLighting)
            {
                mesh.ApplyLighting(64 + brightness, 768 + specular, -50, -10, -50, true);
            }
            
            return mesh;
        }

        public Model GetWidgetMesh()
        {
            return this.GetWidgetMesh(1);
        }

        public Model GetWidgetMesh(int count)
        {
            if (stackIndex != null && count > 1)
            {
                int stack = -1;
                for (int i = 0; i < 10; i++)
                {
                    if (count >= stackAmount[i] && stackAmount[i] != 0)
                    {
                        stack = stackIndex[i];
                    }
                }

                if (stack != -1)
                {
                    return GameContext.Cache.GetItemConfig(stack).GetWidgetMesh();
                }
            }

            Model mesh = new Model(modelIndex);
            if (mesh == null)
            {
                return null;
            }

            if (oldColor != null)
            {
                mesh.SetColors(oldColor, newColor);
            }

            return mesh;
        }

        public Model GetWornMesh(int gender)
        {
            int i1 = maleModel1;
            int i2 = maleModel2;
            int i3 = maleModel3;

            if (gender == 1)
            {
                i1 = femaleModel1;
                i2 = femaleModel2;
                i3 = femaleModel3;
            }

            if (i1 == -1)
            {
                return null;
            }
            
            Model mesh = new Model(i1);

            if (i2 != -1)
            {
                if (i3 != -1)
                {
                    mesh = new Model(3, new Model[] { mesh, new Model(i2), new Model(i3) });
                }
                else
                {
                    mesh = new Model(2, new Model[] { mesh, new Model(i2) });
                }
            }

            if (gender == 0 && maleOffY != 0)
            {
                mesh.Translate(0, maleOffY, 0);
            }

            if (gender == 1 && femaleOffY != 0)
            {
                mesh.Translate(0, femaleOffY, 0);
            }

            if (oldColor != null)
            {
                mesh.SetColors(oldColor, newColor);
            }

            if (particleConfigs != null)
            {
                foreach (var config in particleConfigs)
                {
                    var attachment = new ParticleAttachment(config.vertexIndex);
                    attachment.EmissionRate = (float)config.emissionRate;
                    attachment.GravityModifier = (float)config.gravityModifier;
                    attachment.StartColor = new Color((float)config.r, (float)config.g, (float)config.b, (float)config.a);
                    attachment.StartLifetime = (float)config.startLifetime;
                    attachment.StartSize = (float)config.startSize;
                    attachment.StartSpeed = (float)config.startSpeed;
                    mesh.Attachments.Add(attachment);
                }
            }

            if (index == 7053)
            {
                var attachment = new LightAttachment(13);
                mesh.Attachments.Add(attachment);
            }
            return mesh;
        }

        public bool IsDialogModelValid(int gender)
        {
            int index1 = maleDialogModel1;
            int index2 = maleDialogModel2;

            if (gender == 1)
            {
                index1 = femaleDialogModel1;
                index2 = femaleDialogModel2;
            }

            if (index1 == -1)
            {
                return true;
            }

            bool valid = true;
            return valid;
        }

        public bool IsWornMeshValid(int gender)
        {
            int i1 = maleModel1;
            int i2 = maleModel2;
            int i3 = maleModel3;

            if (gender == 1)
            {
                i1 = femaleModel1;
                i2 = femaleModel2;
                i3 = femaleModel3;
            }

            if (i1 == -1)
            {
                return true;
            }

            return true;
        }
        
        public Texture2D GetTexture(int count, int outlineColor)
        {
            if (stackIndex == null)
            {
                count = -1;
            }

            if (count > 1)
            {
                int i = -1;
                for (int j = 0; j < 10; j++)
                {
                    if (count >= stackAmount[j] && stackAmount[j] != 0)
                    {
                        i = stackIndex[j];
                    }
                }
                if (i != -1)
                {
                    return GameContext.Cache.GetItemConfig(i).GetTexture(count, outlineColor);
                }
            }

            Texture2D noteTexture = null;
            if (noteTemplateIndex != -1)
            {
                noteTexture = GameContext.Cache.GetItemConfig(noteItemIndex).GetTexture(10, -1);
                if (noteTexture == null)
                {
                    return null;
                }
            }

            var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);

            SoftwareRasterizer3D.Texturize = false;
            SoftwareRasterizer2D.Prepare(tex.width, tex.height, new int[32 * 32]);
            SoftwareRasterizer2D.FillRect(0, 0, tex.width, tex.height, 0);
            SoftwareRasterizer3D.Prepare();
            int dist = iconDist;
            if (outlineColor == -1)
            {
                dist = (int)(dist * 1.50D);
            }

            if (outlineColor > 0)
            {
                dist = (int)(dist * 1.04D);
            }

            int sin = MathUtils.Sin[iconPitch] * dist >> 16;
            int cos = MathUtils.Cos[iconPitch] * dist >> 16;


            var model = GetModel(1);
            ModelSoftwareRasterizer.Render(model, 0, iconYaw, iconRoll, iconPitch, iconX, sin + (model.Height / 2) + iconY, cos + iconY);

            var pixels = tex.GetPixels();

            for (int x = tex.width - 1; x >= 0; x--)
            {
                for (int y = tex.height - 1; y >= 0; y--)
                {
                    if (SoftwareRasterizer2D.Pixels[x + y * tex.width] == 0)
                    {
                        if (x > 0 && SoftwareRasterizer2D.Pixels[(x - 1) + y * tex.width] > 1)
                        {
                            SoftwareRasterizer2D.Pixels[x + y * tex.width] = 1;
                        }
                        else if (y > 0 && SoftwareRasterizer2D.Pixels[x + (y - 1) * tex.width] > 1)
                        {
                            SoftwareRasterizer2D.Pixels[x + y * tex.width] = 1;
                        }
                        else if (x < (tex.width - 1) && SoftwareRasterizer2D.Pixels[x + 1 + y * tex.width] > 1)
                        {
                            SoftwareRasterizer2D.Pixels[x + y * tex.width] = 1;
                        }
                        else if (y < (tex.height - 1) && SoftwareRasterizer2D.Pixels[x + (y + 1) * tex.width] > 1)
                        {
                            SoftwareRasterizer2D.Pixels[x + y * tex.width] = 1;
                        }
                    }
                }
            }

            if (outlineColor > 0)
            {
                for (int x = tex.width - 1; x >= 0; x--)
                {
                    for (int y = tex.height - 1; y >= 0; y--)
                    {
                        if (SoftwareRasterizer2D.Pixels[x + y * tex.width] == 0)
                        {
                            if (x > 0 && SoftwareRasterizer2D.Pixels[(x - 1) + y * tex.width] == 1)
                            {
                                SoftwareRasterizer2D.Pixels[x + y * tex.width] = outlineColor;
                            }
                            else if (y > 0 && SoftwareRasterizer2D.Pixels[x + (y - 1) * tex.width] == 1)
                            {
                                SoftwareRasterizer2D.Pixels[x + y * tex.width] = outlineColor;
                            }
                            else if (x < (tex.width - 1) && SoftwareRasterizer2D.Pixels[x + 1 + y * tex.width] == 1)
                            {
                                SoftwareRasterizer2D.Pixels[x + y * tex.width] = outlineColor;
                            }
                            else if (y < (tex.height - 1) && SoftwareRasterizer2D.Pixels[x + (y + 1) * tex.width] == 1)
                            {
                                SoftwareRasterizer2D.Pixels[x + y * tex.width] = outlineColor;
                            }
                        }
                    }
                }
            }
            else if (outlineColor == 0)
            {
                for (int x = tex.width - 1; x >= 0; x--)
                {
                    for (int y = tex.height - 1; y >= 0; y--)
                    {
                        if (SoftwareRasterizer2D.Pixels[x + y * tex.width] == 0 && x > 0 && y > 0 && SoftwareRasterizer2D.Pixels[(x - 1) + (y - 1) * tex.width] > 0)
                        {
                            SoftwareRasterizer2D.Pixels[x + y * tex.width] = 0x302020;
                        }
                    }
                }
            }

            for (int i = 0; i < (tex.width * tex.height); i++)
            {
                var x = i % tex.width;
                var y = tex.height - (i / tex.width) - 1;
                var rpx = (y * tex.width) + x;

                var rgb = SoftwareRasterizer2D.Pixels[i];
                var r = (byte)((rgb >> 16) & 0xFF);
                var g = (byte)((rgb >> 8) & 0xFF);
                var b = (byte)(rgb & 0xFF);
                var a = (byte)255;
                if (rgb == 0)
                {
                    a = 0;
                }
                pixels[rpx] = new Color32(r, g, b, a);
            }

            if (noteTexture != null)
            {
                TextureUtils.Draw(pixels, 32, 32, noteTexture, 0, 0);
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }

    public class ItemConfigProvider : IndexedProvider<ItemConfig>
    {
        private List<ItemConfig> tmpCache = new List<ItemConfig>();
        private int[] pointer;
        private JagexBuffer dataStream;

        public ItemConfigProvider(CacheArchive archive)
        {
            dataStream = new DefaultJagexBuffer(archive.GetFile("obj.dat"));
            var idxStream = new DefaultJagexBuffer(archive.GetFile("obj.idx"));
            pointer = new int[idxStream.ReadUShort()];

            var position = 2;
            for (var i = 0; i < pointer.Length; i++)
            {
                pointer[i] = position;
                position += idxStream.ReadUShort();
            }
        }

        /// <summary>
        /// Removes all excess elements from the cache.
        /// </summary>
        public void RemoveExcess()
        {
            while (tmpCache.Count > 15)
            {
                tmpCache.RemoveAt(0);
            }
        }

        public ItemConfig Provide(int index)
        {
            foreach (var cached in tmpCache)
            {
                if (cached.index == index)
                {
                    return cached;
                }
            }

            if (index < 0 || index >= pointer.Length)
            {
                throw new Exception("Item with index " + index + " does not exist!");
            }
            
            dataStream.Position(pointer[index]);
            var config = new ItemConfig(dataStream);
            config.index = index;
            tmpCache.Add(config);

            RemoveExcess();
            return config;
        }
    }
}