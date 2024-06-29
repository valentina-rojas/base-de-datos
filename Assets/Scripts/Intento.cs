// Clase para mapear la tabla 'intentos' en Supabase
using Postgrest.Models;
using Postgrest.Attributes;

public class intentos : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    public int id_usuario { get; set; }

    public usuarios usuarios { get; set; }

    public int id_category { get; set; }

    public trivia trivia { get; set; }

    [Column("puntaje")]
    public int puntaje { get; set; }
}
