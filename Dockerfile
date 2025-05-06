FROM jupyter/scipy-notebook:7a0c7325e470

# .NET SDK versiyonunu ayarla
ENV DOTNET_SDK_VERSION 9.0.200

# Kullanıcı ve UID ayarları
ARG NB_USER=jovyan
ARG NB_UID=1000
ENV USER ${NB_USER}
ENV NB_UID ${NB_UID}
ENV HOME /home/${NB_USER}

WORKDIR ${HOME}

USER root
RUN apt-get update && apt-get install -y curl

# Install .NET CLI dependencies
RUN apt-get install -y --no-install-recommends \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu60 \
        libssl1.1 \
        libstdc++6 \
        zlib1g

RUN rm -rf /var/lib/apt/lists/*

# Install .NET Core SDK
RUN curl -SL --output dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-x64.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# PATH güncellemesi
ENV PATH="${PATH}:/root/.dotnet/tools"

# .NET Interactive'i yükle
RUN dotnet tool install -g Microsoft.dotnet-interactive
RUN dotnet tool install -g Microsoft.dotnet-try

# Jupyter ile etkileşimi sağlamak için kernel kurulumunu yap
RUN dotnet interactive jupyter install

# Global araçlar için PATH güncellemesi
ENV PATH="${PATH}:/root/.dotnet/tools"

# Notebooks dosyalarını kopyala
COPY ./notebooks/ ${HOME}/notebooks/

# NuGet kaynaklarını kopyala (eğer varsa)
COPY ./NuGet.config ${HOME}/nuget.config

# Kullanıcı sahipliğini ayarla
RUN chown -R ${NB_UID} ${HOME}

# Jupyter'ı kullanıcı olarak çalıştır
USER ${USER}

# Notebook dizinine geç
WORKDIR ${HOME}/notebooks/

# Varsayılan Jupyter portunu aç
EXPOSE 8888

# Jupyter'i başlat
CMD ["start-notebook.sh"]
