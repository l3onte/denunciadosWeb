namespace denunciadosWeb.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public int RolId { get; set; }
    public string Password { get; set; } = String.Empty;

    public Role? Role { get; set; }
}