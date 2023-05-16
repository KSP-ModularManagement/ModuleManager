/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#if CATS
namespace ModuleManager.Cats
{
    class CatAnimator : MonoBehaviour
    {

        public Sprite[] frames;
        public float secFrame = 0.07f;

        private SpriteRenderer spriteRenderer;
        private int spriteIdx;

        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Called by Unity")]
        void Start()
        {
            spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 3;
            StartCoroutine(Animate());
        }


        IEnumerator Animate()
        {
            if (frames.Length == 0)
                yield return null;

            WaitForSeconds yield = new WaitForSeconds(secFrame);

            while (true)
            {
                spriteIdx = (spriteIdx + 1) % frames.Length;
                spriteRenderer.sprite = frames[spriteIdx];
                yield return yield;
            }
        }
    }
}
#endif