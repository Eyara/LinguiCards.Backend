# Observability quickstart

This repository exposes built-in Prometheus metrics and provides an optional
dockerised monitoring stack (Prometheus + Grafana) to visualise them.

## 1. API metrics endpoint

- Metrics are exposed from `LinguiCards.API` at `GET /metrics`.
- The endpoint is automatically enabled when the application starts.
- Default histograms/counters are provided by `prometheus-net` via
  `app.UseHttpMetrics()`; add your own collectors if you need domain-specific
  data.

## 2. Local monitoring stack

The `monitoring/` directory includes everything needed to spin up Prometheus
and Grafana locally.

```bash
docker compose -f monitoring/docker-compose.monitoring.yml up -d
```

What you get:

- Prometheus on http://localhost:9090
- Grafana on http://localhost:3000 (user/password: `admin` / `admin`)

> **Note:** Prometheus is configured to scrape `host.docker.internal:8080`.
> If your API runs elsewhere, edit
> `monitoring/prometheus/prometheus.yml` accordingly.

Grafana automatically provisions a Prometheus data source and the dashboard
stored at `monitoring/grafana/dashboards/linguicards-api-overview.json`.

## 3. Production hints

- Expose `/metrics` over the internal network only and secure with network ACLs.
- Point Prometheus at the production endpoint by updating the scrape target.
- Version control additional Grafana dashboards inside
  `monitoring/grafana/dashboards/`.
- Consider pushing container metrics (cAdvisor/node_exporter) to Prometheus and
  overlaying them on the provided dashboard.

