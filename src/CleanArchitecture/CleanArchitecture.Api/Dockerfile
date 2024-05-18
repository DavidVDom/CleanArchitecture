# la imagen del runtime de .Net del que se necesitará imagen
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# dentro del container se crearé directorio app y el runtime se instalará ahí
WORKDIR /app
# puerto del container de la aplicación
EXPOSE 80

# este puerto a nivel interno, el anterior era de cara a los clientes, se comenta porque se haré en el docker compose
# ENV ASPNETCORE_URLS=http://+:5000

# como no es bbdd ni servicio a administrar no necesito un usuario
# USER app
# se necesita el sdk para compilar la aplicación
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# argumento de compilación, en vez de release querremos hacer debug dentro del container posteriormente
# ARG configuration=Release
ARG configuration=Debug
# directorio dentro del container
WORKDIR /src
# el copy (resultado de la compilación) lo hará dentro del workdir src. Sólo nos crea el de la API, pero aunque no son necesarios, no está de más añadir los otros proyectos
COPY ["src/CleanArchitecture/CleanArchitecture.Domain/CleanArchitecture.Domain.csproj", "src/CleanArchitecture/CleanArchitecture.Domain/"]
COPY ["src/CleanArchitecture/CleanArchitecture.Application/CleanArchitecture.Application.csproj", "src/CleanArchitecture/CleanArchitecture.Application/"]
COPY ["src/CleanArchitecture/CleanArchitecture.Infrastructure/CleanArchitecture.Infrastructure.csproj", "src/CleanArchitecture/CleanArchitecture.Infrastructure/"]
COPY ["src/CleanArchitecture/CleanArchitecture.Api/CleanArchitecture.Api.csproj", "src/CleanArchitecture/CleanArchitecture.Api/"]
# el restore descarga los Nuget que se estén usando
RUN dotnet restore "src/CleanArchitecture/CleanArchitecture.Api/CleanArchitecture.Api.csproj"
COPY . .
# el primer src es el del docker container, dentro creamos otro src donde irán las aplicaciones
WORKDIR "/src/src/CleanArchitecture/CleanArchitecture.Api"
RUN dotnet build "CleanArchitecture.Api.csproj" -c $configuration -o /app/build

# lo publica
FROM build AS publish
# ARG configuration=Release
ARG configuration=Debug
RUN dotnet publish "CleanArchitecture.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ese binario (dll) será el que levante la aplicación, el entrypoint
ENTRYPOINT ["dotnet", "CleanArchitecture.Api.dll"]
