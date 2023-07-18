public class BigExp : Exp
{
    protected override void Get()
    {
        base.Get();
        PoolManager.Instance.ActionObjects("Exp", (obj) => { obj.GetComponent<Exp>().OnGet(); });
    }
}