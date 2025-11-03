namespace DockQueue.Client.Authentication;

public class UserSession
{
    public int Id                       { get; set; }
    public string Nome                  { get; set; }
    public string Codigo                { get; set; }
	public string Descricao             { get; set; }
	public string UsuarioAcesso         { get; set; }
    public string Permissoes            { get; set; }
    public string JsonPermissoes        { get; set; }
    public string ChaveCliente          { get; set; }
    public string Cnpj                  { get; set; }
    public bool IsAdmin                 { get; set; }
    public bool IsUser                  { get; set; }
    public bool IsRepresentante         { get; set; }
    public string CodigoRepresentante   { get; set; }
}