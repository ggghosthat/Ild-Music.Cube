using ShareInstances.Instances;
using Cube.Mapper.Entities;

using System.Linq;
using System.Data.SQLite;
using Dapper;
namespace Cube.Storage;
public class Engine
{
    private string path;
    private ReadOnlyMemory<char> _connectionString;

    public Engine(string path)
    {
        this.path = path;
        this._connectionString = $"Data Source = {this.path}".AsMemory();
    }

    public void StartEngine()
    {
        try
        {
            if(!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute("create table if not exists artists(Id integer primary key, AID text, Name varchar, Description varchar, Avatar blob)");
                connection.Execute("create table if not exists playlists(Id integer primary key, PID text, Name varchar, Description varchar, Avatar blob)");
                connection.Execute("create table if not exists tracks(Id integer primary key, TID text, Path varchar, Name varchar, Description varchar, Avatar blob, Valid integer, Duration integer)");
                

                connection.Execute("create table if not exists artists_playlists(Id integer primary key, AID text, PID text)");
                connection.Execute("create table if not exists artists_tracks(Id integer primary key, AID text, TID text)");
                connection.Execute("create table if not exists playlists_tracks(Id integer primary key, PID text, TID text)");


                connection.Execute("create table if not exists tags(Id integer primary key, TagID text, Name text, Color text)");
                connection.Execute("create table if not exists tags_instances(Id integer primary key, TagID text, IID text)");
            }
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public void Add<T>(T entity)
    {
        ReadOnlySpan<char> dapperQuery;
        if (entity is ArtistMap artist)
        {
            dapperQuery = "insert or ignore into artists(AID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {                
                connection.Execute(dapperQuery.ToString(), artist);     
            }
        }
        else if (entity is PlaylistMap playlist)
        {
            dapperQuery = "insert or ignore into playlists(PID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), playlist );
            }
        }
        else if (entity is TrackMap track)
        {
            dapperQuery = "insert or ignore into tracks(TID, Path, Name, Description, Avatar, Valid, Duration) values (@Buid, @Pathway, @Name, @Description, @Avatar, @IsValid, @Duration)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), track);
            }
        }
        else if (entity is TagMap tag)
        {
            dapperQuery = "insert or ignore into tags(TagID, Name, Color) values (@Buid, @Name, @Color)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), tag);
            }
        }
    }

    public void AddStores(ICollection<Store> stores)
    {
        stores.ToList().ForEach(store => 
        {   
            ReadOnlySpan<char> dapperQuery;
            switch(store.Tag)
            {
                case(1):
                    dapperQuery = "insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=store.Holder.ToString(), pid=relate.ToString() });
                        }
                    }
                    break;
                case(2):
                    dapperQuery = "insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=store.Holder.ToString(), tid=relate.ToString() });
                        }
                    }
                    break;
                case(3):
                    dapperQuery = "insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=relate.ToString(), pid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(4):
                    dapperQuery = "insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {pid=store.Holder.ToString(), tid=relate.ToString() });
                        }
                    }
                    break;
                case(5):
                    dapperQuery = "insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=relate.ToString(), tid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(6):
                    dapperQuery = "insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {pid=relate.ToString(), tid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(7):
                    dapperQuery = "insert into tags_instances(TagID, IID) select @tagid, @iid where not EXISTS(SELECT 1 from tags_instances where TagID = @tagid and IID = @iid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {tagid=relate.ToString(), iid=store.Holder.ToString() });
                        }
                    }
                    break;
                default:break;
            }
        });
    }

    public void Edit<T>(T entity)
    {
        ReadOnlySpan<char> dapperQuery;
        if (entity is ArtistMap artist)
        {
            dapperQuery = "update artists set Name = @Name, Description = @Description, Avatar = @Avatar where AID = @Buid".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), artist);
            }
        }
        else if (entity is PlaylistMap playlist)
        {
            dapperQuery = "update playlists set Name = @Name, Description = @Description, Avatar = @Avatar where PID = @Buid".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), playlist);
            }

        }
        else if (entity is TrackMap track)
        {
            dapperQuery = "update tracks set Path = @Pathway, Name = @Name, Description = @Description, Avatar = @Avatar, Valid = @IsValid, Duration = @Duration where TID = @Buid".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), track);
            }

        }
    }

    public void EditStores(ICollection<Store> stores)
    {
       stores.ToList().ForEach(store => 
        {   
            ReadOnlySpan<char> dapperQuery;
            switch(store.Tag)
            {
                case(1):
                    dapperQuery = @"delete from artists_playlists where AID = @aid;
                                   insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {aid=store.Holder.ToString(), pid=relate.ToString() });
                        }
                    }
                    break;
                case(2):
                    dapperQuery = @"delete from artists_tracks where AID = @aid;
                                    insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {aid=store.Holder.ToString(), tid=relate.ToString() });
                        }
                    }
                    break;
                case(3):
                    dapperQuery = @"delete from artists_playlists where PID = @pid;
                                    insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {aid=relate.ToString(), pid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(4):
                    dapperQuery = @"delete from playlists_tracks where PID = @pid;
                                    insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {pid=store.Holder.ToString(), tid=relate.ToString() });
                        }
                    }
                    break;
                case(5):
                    dapperQuery = @"delete from artists_tracks where TID = @tid;
                                    insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {aid=relate.ToString(), tid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(6):
                    dapperQuery = @"delete from playlists_tracks where TID = @tid;
                                    insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {pid=relate.ToString(), tid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(7):
                    dapperQuery = @"delete from tags_instances where IID = @iid;
                                    insert into tags_instances(TagID, IID) select @tagid, @iid where not EXISTS(SELECT 1 from tags_instances where TagID = @tagid and IID = @iid);".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.QueryMultiple(dapperQuery.ToString(), new {tagid=relate.ToString(), iid=store.Holder.ToString() });
                        }
                    }
                    break;
                default:break;
            }
        });
 
    }

    public void Delete<T>(ref T entity)
    {
        ReadOnlySpan<char> dapperQuery;
        if(entity is Artist artist)
        {
            dapperQuery = @"delete from artists where AID = @aid;
                            delete from artists_playlists where AID = @aid;
                            delete from artists_tracks where AID = @aid;".AsSpan();

            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.QueryMultiple(dapperQuery.ToString(), new {aid=artist.Id.ToString()});
            }
        }
        else if(entity is Playlist playlist)
        {
            dapperQuery = @"delete from playlists where PID = @pid;
                            delete from artists_playlists where PID = @pid;
                            delete from playlists_tracks where PID = @pid;".AsSpan();

            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.QueryMultiple(dapperQuery.ToString(), new {pid=playlist.Id.ToString()});
            }
        }
        else if(entity is Track track)
        {
            dapperQuery = @"delete from tracks where TID = @tid;
                            delete from playlists_tracks where TID = @tid;
                            delete from artists_tracks where TID = @tid;".AsSpan();

            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.QueryMultiple(dapperQuery.ToString(), new {tid=track.Id.ToString()});
            }

        }
    }

    public (IEnumerable<ArtistMap>, IEnumerable<PlaylistMap>, IEnumerable<TrackMap>) BringAll(int offset, int capacity)
    {
        IEnumerable<ArtistMap> artists;
        IEnumerable<PlaylistMap> playlists;
        IEnumerable<TrackMap> tracks;

        ReadOnlySpan<char> dapperQuery = @"
        select AID, Name, Description, Avatar from artists where Id >= @offset and Id <= @capacity;
        select PID, Name, Description, Avatar from playlists where Id >= @offset and Id <= @capacity;
        select TID, Pathway, Name, Description, Avatar, Valid, Duration from tracks where Id >= @offset and Id <= @capacity;".AsSpan();

        using (var connection = new SQLiteConnection(_connectionString.ToString()))
        {
            using (var multiQuery = connection.QueryMultiple(dapperQuery.ToString(), new {offset=offset, capacity=capacity}))
            {
                artists = multiQuery.Read<ArtistMap>();
                playlists = multiQuery.Read<PlaylistMap>();
                tracks = multiQuery.Read<TrackMap>();
            }
        }
        return (artists, playlists, tracks);
    }

    public IEnumerable<T> Bring<T>(int offset, int capacity)
    {
        ReadOnlySpan<char> dapperQuery;
        IEnumerable<T> result;
        if(typeof(T) == typeof(ArtistMap))
        {
            dapperQuery = "select AID, Name, Description, Avatar from artists where Id >= @offset and Id <= @capacity".AsSpan();
        }
        else if(typeof(T) == typeof(PlaylistMap))
        {
            dapperQuery = "select PID, Name, Description, Avatar from playlists where Id >= @offset and Id <= @capacity".AsSpan();
        }
        else if(typeof(T) == typeof(TrackMap))
        {
            dapperQuery = "select TID, Pathway, Name, Description, Avatar, Valid, Duration from tracks where Id >= @offset and Id <= @capacity".AsSpan();
        }
        else if(typeof(T) == typeof(TagMap))
        {
            dapperQuery = "select TagID, Name, Color from tags where Id >= @offset and Id <= @capacity".AsSpan();
        }
        else return null;

        using (var connection = new SQLiteConnection(_connectionString.ToString()))
        {
            result = connection.Query<T>(dapperQuery.ToString(), new {offset=offset, capacity=capacity});
        }
        return result;
    }

    public Store BringStores(int tag, Guid id)
    {
        ReadOnlySpan<char> dapperQuery;
        Store store = new Store(tag:tag);
        switch(tag)
        {
            case (1):
                dapperQuery = @"select AID, PID from artists_playlists where AID = @id;".AsSpan();
                var apPairs = PairsObtain<ApPair>(dapperQuery, id);
                store.Holder = new Guid(apPairs.First().AID);
                store.Relates = apPairs.Select(x => new Guid(x.PID)).ToList();
                break;
            case (2):
                dapperQuery = @"select PID, AID from artists_playlists where PID = @id;".AsSpan();
                var paPairs = PairsObtain<ApPair>(dapperQuery, id);
                store.Holder = new Guid(paPairs.First().PID);
                store.Relates = paPairs.Select(x => new Guid(x.AID)).ToList();
                break;
            case (3):
                dapperQuery = @"select AID, TID from artists_tracks where AID = @id;".AsSpan();
                var atPairs = PairsObtain<AtPair>(dapperQuery, id);
                store.Holder = new Guid(atPairs.First().AID);
                store.Relates = atPairs.Select(x => new Guid(x.TID)).ToList();
                break;
            case (4):
                dapperQuery = @"select TID, AID from artists_tracks where TID = @id;".AsSpan();
                var taPairs = PairsObtain<AtPair>(dapperQuery, id);
                store.Holder = new Guid(taPairs.First().TID);
                store.Relates = taPairs.Select(x => new Guid(x.AID)).ToList();
                break;
            case (5):
                dapperQuery = @"select PID, TID from playlists_tracks where PID = @id;".AsSpan();
                var ptPairs = PairsObtain<PtPair>(dapperQuery, id);
                store.Holder = new Guid(ptPairs.First().PID);
                store.Relates = ptPairs.Select(x => new Guid(x.TID)).ToList();
                break;
            case (6):
                dapperQuery = @"select TID, PID from playlists_tracks where TID = @id;".AsSpan();
                var tpPairs = PairsObtain<PtPair>(dapperQuery, id);
                store.Holder = new Guid(tpPairs.First().TID);
                store.Relates = tpPairs.Select(x => new Guid(x.PID)).ToList();
                break;
            default: break;
        }

        return store;
    }

    private IEnumerable<T> PairsObtain<T>(ReadOnlySpan<char> dapperQuery, Guid id)
    {
        if(typeof(T) == typeof(ApPair) || typeof(T) == typeof(AtPair) || typeof(T) == typeof(PtPair))
        {
            IEnumerable<T> obtained;
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                obtained = connection.Query<T>(dapperQuery.ToString(), new {id=id.ToString()} );            
            }
            
            return obtained;
        }
        else throw new Exception("Not supported Pair type. There are only support for ap, at and pt pair type.");
    }
}
