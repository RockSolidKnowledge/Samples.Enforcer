FROM kong/kong-gateway:3.10
# Ensure any patching steps are executed as root user
USER root

# Add custom plugin to the image
COPY ./kong/plugins/authzen /usr/local/share/lua/5.1/kong/plugins/authzen
ENV KONG_PLUGINS=bundled,authzen

# Ensure kong user is selected for image execution
USER kong

# Run kong
ENTRYPOINT ["/entrypoint.sh"]
EXPOSE 8000 8001 8002 8003 8004 8443 8444 8445
STOPSIGNAL SIGQUIT
HEALTHCHECK --interval=10s --timeout=10s --retries=10 CMD kong health
CMD ["kong", "docker-start"]
