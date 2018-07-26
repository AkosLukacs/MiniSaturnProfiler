namespace Books

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks
open FSharp.Control.Tasks.ContextInsensitive
open StackExchange.Profiling.Data
open StackExchange.Profiling

module Database =
  let getConnection connectionString = new ProfiledDbConnection(new SqliteConnection(connectionString), MiniProfiler.Current)

  let getAll connectionString : Task<Result<Book seq, exn>> =
    task {
      use connection = getConnection connectionString
      return! query connection "SELECT id, title, author FROM Books" None
    }

  let getById connectionString id : Task<Result<Book option, exn>> =
    task {
      use connection = getConnection connectionString
      return! querySingle connection "SELECT id, title, author FROM Books WHERE id=@id" (Some <| dict ["id" => id])
    }

  let update connectionString v : Task<Result<int,exn>> =
    task {
      use connection = getConnection connectionString
      return! execute connection "UPDATE Books SET id = @id, title = @title, author = @author WHERE id=@id" v
    }

  let insert connectionString v : Task<Result<int,exn>> =
    task {
      use connection = getConnection connectionString
      return! execute connection "INSERT INTO Books(id, title, author) VALUES (@id, @title, @author)" v
    }

  let delete connectionString id : Task<Result<int,exn>> =
    task {
      use connection = getConnection connectionString
      return! execute connection "DELETE FROM Books WHERE id=@id" (dict ["id" => id])
    }

