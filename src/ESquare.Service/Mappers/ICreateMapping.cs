using AutoMapper;

namespace ESquare.Service.Mappers
{
    public interface ICreateMapping
    {
        /// <summary>
        /// Defines the mapping and adds it into the mapping profile
        /// </summary>
        /// <param name="profile"></param>
        void CreateMapping(Profile profile);
    }
}
