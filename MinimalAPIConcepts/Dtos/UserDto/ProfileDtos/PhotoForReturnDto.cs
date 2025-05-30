namespace NEXT.GEN.Dtos.UserDto.ProfileDtos;

public class PhotoForReturnDto
{
    public int Id { set; get; }
    public string Url { get; set; }
    public string Description { set; get; }
    public DateTime DateAdded { set; get; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }
}