
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    /// <summary>
    /// 付箋エディタの出し入れを担当します。
    /// </summary>
    public class StickerEditorManager : UdonSharpBehaviourWithUtils
    {
        private const int MaxClosedEditorCount = 4;
        [SerializeField] private StickerEditor m_editorPrefab;
        [SerializeField] private Transform m_editorParent;

        private System m_system;

        public void Setup(System system)
        {
            m_system = system;
        }

        public StickerEditor FindOrCreateEditor(string stickerId, string content, Color color, Vector3 position, Quaternion rotation, StickerEditorViewMode initialViewMode)
        {
            var objectName = GetEditorObjectNameByStickerId(stickerId);
            var tf = m_editorParent.Find(objectName);
            if (tf != null)
            {
                var editor = tf.GetComponent<StickerEditor>();
                if (editor != null)
                {
                    editor.Setup(stickerId, content, color, position, rotation, m_system, initialViewMode, this, nameof(OnCloseEditor));
                    return editor;
                }
                else
                {
                    // 同名のオブジェクトがあるが、動作していないので消す
                    Destroy(tf.gameObject);
                }
            }

            // 未使用のオブジェクトがあれば再利用する
            for (var childIndex = m_editorParent.childCount - 1; childIndex >= 0; childIndex--)
            {
                tf = m_editorParent.GetChild(childIndex);
                if (tf == null) continue;
                if (tf.gameObject.activeSelf) continue;

                var editor = tf.GetComponent<StickerEditor>();
                if (editor == null)
                {
                    // StickerEditor以外のオブジェクトがあるので消す
                    Destroy(tf.gameObject);
                    continue;
                }

                // 使えるモノが見つかったのでこれを使う
                tf.gameObject.name = objectName;
                editor.Setup(stickerId, content, color, position, rotation, m_system, initialViewMode, this, nameof(OnCloseEditor));
                return editor;
            }

            // 新しく作る
            {
                var editorGameObject = Instantiate(m_editorPrefab.gameObject, m_editorParent, false);
                var editor = editorGameObject.GetComponent<StickerEditor>();
                if (editor == null)
                {
                    Log("付箋エディタPrefabが壊れています！");
                    return null;
                }
                editorGameObject.name = objectName;
                editor.Setup(stickerId, content, color, position, rotation, m_system, initialViewMode, this, nameof(OnCloseEditor));
                return editor;
            }
        }

        public void OnCloseEditor()
        {
            // Closeされているエディタ数が規定数を超えていたら削除する
            var currentClosedEditorCount = 0;
            for (var childIndex = m_editorParent.childCount - 1; childIndex >= 0; childIndex--)
            {
                var tf = m_editorParent.GetChild(childIndex);
                if (tf == null) continue;
                if (tf.gameObject.activeSelf) continue;

                if (currentClosedEditorCount < MaxClosedEditorCount)
                {
                    // 規定数以内なので削除しない
                    currentClosedEditorCount++;
                    continue;
                }

                // 規定数を超えているので削除する
                Destroy(tf.gameObject);
            }
        }

        private string GetEditorObjectNameByStickerId(string stickerId)
        {
            return "Editor_" + stickerId;
        }
    }

}