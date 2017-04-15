public interface IPoolableObject
{
    bool isPooled { get; set; }
    bool canReturnToPool { get; }

    void DisableObject();
}