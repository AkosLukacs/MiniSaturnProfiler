module Server

open Saturn
open Config
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder

let endpointPipe = pipeline {
    plug head
    plug requestId
}

let app = application {
    pipe_through endpointPipe

    error_handler (fun ex _ -> pipeline { render_html (InternalError.layout ex) })
    use_router Router.appRouter
    url "http://0.0.0.0:8085/"
    memory_cache
    use_static "static"
    use_gzip
    use_config (fun _ -> {connectionString = "DataSource=database.sqlite"} ) //TODO: Set development time configuration

    app_config(fun app ->
        printfn "@app_config..."

        app.UseMiniProfiler() |> ignore
        app
    )
    service_config(fun services ->
        printfn "@service_config..."

        // required by Miniprofiler in-memory storage
        services.AddMemoryCache() |> ignore
        services.AddMiniProfiler() |> ignore

        services
    )


}

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
    run app
    0 // return an integer exit code