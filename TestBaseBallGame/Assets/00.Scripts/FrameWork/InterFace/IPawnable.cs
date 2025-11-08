public interface IPawnable<T> where T : IPawnable<T>
{
    void SetController(Controller controller);
}
