# TvMaze

## Development

- Visual Studio Community 2019 (version 16.7.6)
- Docker Desktop Community (version 2.5.0.0, engine 19.03.13)

## Design

The solution consists of multiple components:
- `TVShowsController`, the api controller exposing the the get method on /api/tvshows?page=0
- `GetPaginatedTVShowsCommand`, the command which encapsulate the get method
- Repositories and UnitOfWork
- EF Model
- SQLite database. For the sake of simplicity the database file will be copied to the image and will be lost each time container is restarted. This has been done deliberately.
- `TvMazeScraper`, scrape api.tvmaze.com, it poll the api every 10 minutes and update DB accordingly(only missing items will be added). It stores the data to database per movie as soon as movie and cast are collected.
- HostedService, host the scraper

## HowToRun(From the root of the git repository)

- Create SQlite database
```
(git repository root)
cd ./Maze
dotnet ef database update
```
- Build docker image
```
(git repository root)
docker build --tag tvmaze .
```
- Run the docker container
```
docker run --publish 8080:80 --detach --name tvmazeapp tvmaze
```
or, interactive mode, handy to inspect logs
```
docker run -it --rm -e "ASPNETCORE_ENVIRONMENT=Development" -p 8080:80 --name tvmazeapp tvmaze
```
- Open browser and try to hit the api endpoint. Please notice that data will be stored per movie instead of waiting to scrape all the movies to improve user experience.
```
localhost:8080/api/tvshows?page=0
```

## Database

The model has been many-to-many relationship between TVShow and Actor with JoinedTable.

| TVShow | ---< | TVShowActor | >--- | Actor |

I have added just a minimal number of properties as specified, more properties can be added on database and deserialization part.

## HttpRateLimiter

The Http rate limiter has been implemented to protect agains api.tvmaze.com limitation. Each time TooManyRequests Http response is received the rate limiter is notified and it increase the timeout guard by 1 second up until 10. In order to be resiliant to these errors additional retry mechanism has been added.

## Testing

I didn't write too many unit tests, mostly tested business requirements and some tricky part like retries. More unit tests should be definetelly added. Logging has been also added, it can be inspected at Logging-<timestamp>.log in the app directory. As a side note, I have mostly focus on the basic functionality and didn't spend too much time on logging and exception handling. The current state of the solution should give impression that awarness is there and it should be definetelly improved.
