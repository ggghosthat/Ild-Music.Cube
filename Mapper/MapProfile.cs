using ShareInstances.Instances;
using Cube.Mapper;
using Cube.Mapper.Entities;

using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;

namespace Cube.Mapper;
public sealed class MapProfile : Profile
{    
    public MapProfile()
    {
        CreateMap<Artist, ArtistMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.ToString()))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarSource.ToArray()));

        CreateMap<Playlist, PlaylistMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.ToString()))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarSource.ToArray()));

        CreateMap<Track, TrackMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Pathway, opt => opt.MapFrom(src => src.Pathway.ToString() ))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.ToString()))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarSource.ToArray()))
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid?1:0))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration.TotalMilliseconds));

        CreateMap<Tag, TagMap>()
            .ForMember(dest => dest.Buid, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()));

        CreateMap<ArtistMap, Artist>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Guid(src.Buid) ))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.AsMemory() ))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.AsMemory() ))
            .ForMember(dest => dest.AvatarSource, opt => opt.MapFrom(src => src.Avatar.AsMemory() ));

        CreateMap<PlaylistMap, Playlist>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Guid(src.Buid) ))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.AsMemory() ))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.AsMemory() ))
            .ForMember(dest => dest.AvatarSource, opt => opt.MapFrom(src => src.Avatar.AsMemory() ));

        CreateMap<TrackMap, Track>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Guid(src.Buid) ))
            .ForMember(dest => dest.Pathway, opt => opt.MapFrom(src => src.Pathway.AsMemory() ))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.AsMemory() ))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.AsMemory() ))
            .ForMember(dest => dest.AvatarSource, opt => opt.MapFrom(src => src.Avatar.AsMemory() ))
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => (src.IsValid == 1)?true:false ))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.FromMilliseconds(src.Duration) ));

        CreateMap<ICollection<Guid>, Store>()
            .ConvertUsing((src) => GenerateStore(src));

        CreateMap<ICollection<Track>, Store>()
            .ConvertUsing((src) => GenerateStore(src));
            
        CreateMap<ICollection<Tag>, Store>()
            .ConvertUsing((src) => GenerateStore(src));

        CreateMap<Store, ICollection<Guid>>()
            .ConvertUsing((src) => GenerateCollection(src));
    }

    #region Private Methods
    private Store GenerateStore(ICollection<Guid> items)
    {
        var store = new Store(0);
        items.ToList().ForEach(i =>
        {
            store.Relates.Add(i);
        });

        return store;
    }

    private Store GenerateStore(ICollection<Track> tracks)
    {
        var store = new Store(0);
        tracks.ToList().ForEach(t =>
        {
            store.Relates.Add(t.Id);
        });

        return store;
    }

    private Store GenerateStore(ICollection<Tag> tags)
    {
        var store = new Store(7);
        tags.ToList().ForEach(t =>
        {
            store.Relates.Add(t.Id);
        });

        return store;
    }

    private ICollection<Guid> GenerateCollection(Store store)
    {
        return store.Relates;
    }    
    #endregion 
}
