using Postgrest.Models;
using Postgrest.Attributes;

public class intentos : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("id_usuario")]
    public int id_usuario { get; set; }

    [Column("id_category")]
    public int id_category { get; set; }

    [Column("puntaje")]
    public int puntaje { get; set; }

    [Column("tiempoJuego")]
    public int tiempoJuego { get; set; }

    [Column("respCorrectas")]
    public int respCorrectas { get; set; }

    [Column("respIncorrectas")]
    public int respIncorrectas { get; set; }

}
