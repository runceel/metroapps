
namespace ImageProcessingSample.Media.Effects
{
    /// <summary>
    /// エフェクト処理をおこなうメソッドを定義します
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// エフェクト処理をおこなう
        /// </summary>
        /// <param name="width">ビットマップの幅</param>
        /// <param name="height">ビットマップの高さ</param>
        /// <param name="source">byte配列のビットマップ</param>
        /// <returns>処理後のbyte配列のビットマップ</returns>
        byte[] Effect(int width, int height, byte[] source);
    }
}
