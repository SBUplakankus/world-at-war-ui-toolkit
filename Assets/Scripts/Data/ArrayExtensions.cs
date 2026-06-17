namespace Data
{
    public static class ArrayExtensions
    {
        public static T Random<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
    }
}
