FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Yarn's Debian repo key has expired (EXPKEYSIG ...) and breaks apt-get update.
# Remove the Yarn apt source so devcontainer features (common-utils) can install.
RUN rm -f /etc/apt/sources.list.d/yarn.list \
    && rm -f /usr/share/keyrings/yarnkey.gpg \
    && rm -f /etc/apt/trusted.gpg.d/yarn.gpg