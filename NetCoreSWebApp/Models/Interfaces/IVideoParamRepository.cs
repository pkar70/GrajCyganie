namespace NetCoreSWebApp.Models
{
    public interface IVideoParamRepository
    {
        // FILMY.asp
        // sInnerJoin = " FROM StoreFiles LEFT JOIN videoParam ON StoreFiles.ID = videoParam.fileID "

        // FILMYAKTORA.asp
        // sInnerJoin = " FROM StoreFiles INNER JOIN videoParam ON StoreFiles.ID = videoParam.fileID "
        // sQuery = "SELECT COUNT (ID),SUM(duration) FROM videoParam"

        int GetTotalCount();
        int GetTotalDuration();

        VideoParam? GetById(int videoParamId);

    }
}
