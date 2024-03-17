/*
	This file is part of Module Manager /L
		© 2018-2024 LisiasT
		© 2013-2018 Sarbian; Blowfish
		© 2013 ialdabaoth

	Module Manager /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Module Manager /L. If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using UnityEngine;
using K = KSPe.Util.Log;

#if CATS
namespace ModuleManager.Cats
{
    public static class CatManager
    {
        private static Sprite[] catFrames;
        private static Texture2D rainbow;
        private static int scale = 1;

        public static void LaunchCat()
        {
            InitCats();

            GameObject cat = LaunchCat(scale);
            cat.AddComponent<CatMover>();
        }

        public static void LaunchCats()
        {
            InitCats();

            GameObject catSun = LaunchCat(scale);
            CatOrbiter catSunOrbiter = catSun.AddComponent<CatOrbiter>();
            catSunOrbiter.Init(null, 0);

            int cats = UnityEngine.Random.Range(6, 10);
            for (int i = 0; i < cats; i++)
            {
                GameObject cat = LaunchCat(scale);
                CatOrbiter catOrbiter = cat.AddComponent<CatOrbiter>();
                catOrbiter.Init(catSunOrbiter, Screen.height * 0.5f);

                int moons = UnityEngine.Random.Range(0, 4);

                for (int j = 0; j < moons; j++)
                {
                    GameObject catMoon = LaunchCat(scale);
                    CatOrbiter catMoonOrbiter = catMoon.AddComponent<CatOrbiter>();
                    catMoonOrbiter.Init(catOrbiter, Screen.height * 0.06f);
                }
            }
        }

        private static void InitCats()
        {
            Texture2D[] tex = new Texture2D[12];
            for (int i = 0; i < tex.Length; i++)
            {
                tex[i] = new Texture2D(70, 42, TextureFormat.ARGB32, false);
			}
			{
				int i = 0;
				tex[i++].LoadRawTextureData(Properties.Resources.cat1);
				tex[i++].LoadRawTextureData(Properties.Resources.cat2);
				tex[i++].LoadRawTextureData(Properties.Resources.cat3);
				tex[i++].LoadRawTextureData(Properties.Resources.cat4);
				tex[i++].LoadRawTextureData(Properties.Resources.cat5);
				tex[i++].LoadRawTextureData(Properties.Resources.cat6);
				tex[i++].LoadRawTextureData(Properties.Resources.cat7);
				tex[i++].LoadRawTextureData(Properties.Resources.cat8);
				tex[i++].LoadRawTextureData(Properties.Resources.cat9);
				tex[i++].LoadRawTextureData(Properties.Resources.cat10);
				tex[i++].LoadRawTextureData(Properties.Resources.cat11);
				tex[i++].LoadRawTextureData(Properties.Resources.cat12);
			}
            rainbow = new Texture2D(39, 36, TextureFormat.ARGB32, false);
            rainbow.LoadRawTextureData(Properties.Resources.rainbow);
            rainbow.Apply();

            catFrames = new Sprite[12];

            for (int i = 0; i < tex.Length; i++)
            {
                tex[i].Apply();
                catFrames[i] = Sprite.Create(tex[i], new Rect(0, 0, tex[i].width, tex[i].height), new Vector2(.5f, .5f));
                catFrames[i].name = "cat" + i;
            }

            scale = 1;
            if (Screen.height >= 1080)
                scale *= 2;
            if (Screen.height > 1440)
                scale *= 3;

            Physics2D.gravity = Vector2.zero;
        }

        private static GameObject LaunchCat(int scale)
        {
            GameObject cat = new GameObject("NyanCat");
            SpriteRenderer sr = cat.AddComponent<SpriteRenderer>();
            TrailRenderer trail = cat.AddComponent<TrailRenderer>();
            CatAnimator catAnimator = cat.AddComponent<CatAnimator>();

            sr.sprite = catFrames[0];

            trail.material = new Material(Shader.Find(
                2019 == KSPe.Util.UnityTools.UnityVersion
                    ? "Legacy Shaders/Particles/Alpha Blended"
                    : "Particles/Alpha Blended"
                ));

            LOG.info("material = {0}", trail.material);
            trail.material.mainTexture = rainbow;
            trail.time = 1.5f;
            trail.startWidth = 0.6f * scale * rainbow.height;

            trail.endColor = Color.white.A(0.1f);
            trail.colorGradient = new Gradient {alphaKeys = new GradientAlphaKey[3] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 0.75f), new GradientAlphaKey(0.2f, 1)  }};
            trail.Clear();
            cat.layer = LayerMask.NameToLayer("UI");

            catAnimator.frames = catFrames;

            cat.transform.localScale = 70 * scale * Vector3.one;
            return cat;
        }

        internal static readonly K.Logger LOG = K.Logger.CreateForType<CatAnimator>(true);
   }
}
#endif