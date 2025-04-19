# Campus Pathfinder AI – Executable Build Plan (v0.2)
_Target submission: **May 1 2025 02:59 EDT**_

## 0 — Project Synopsis
Campus Pathfinder AI is a **multi‑agent concierge** for higher‑ed institutions, answering and escalating student questions via SMS, web chat, and voice. It uses Azure AI Agent Service (Planner → RAG Expert → Action), Semantic Kernel reflection, Twilio for v1 telephony, and Azure AI Search + Cosmos DB as its knowledge base.

---

## Conventions
- ✅ = done 🟡 = in progress ❌ = blocked  
- **DoD** = “Definition of Done” – objective acceptance criteria  
- **All tasks are owned by ME.**

---

## 1. Project Bootstrap (Status 🟡)
| # | Task | DoD |
|---|------|-----|
|1.1|✅ Create public repo `campus‑pathfinder‑ai` | Repo visible on GitHub |
|1.2|✅ Add MIT LICENSE & `.gitignore` | Files committed on `main` |
|1.3|✅ Add `README.md` scaffold | Shows project synopsis & badge placeholder |
|1.4|✅  Add .NET Aspire/Minimal API solution skeleton (`/src/Server`, `/src/Agents`) | `dotnet build` succeeds locally |
|1.5|✅ Add GitHub Actions CI (build + test) | Green check on first run |
|1.6|✅  Commit this `BUILD_PLAN.md` to repo | Plan visible on GitHub |

---

## 2. Data Layer & RAG Pipeline (Apr 18–19)
| # | Task | DoD |
|---|------|-----|
|2.1|❌ Design sample knowledge schema (FAQ, catalog) as JSON files in `/data/raw` | Schema documented in `README` |
|2.2|❌ Write `ingest_catalog.csx` Azure Function to push data to Cosmos DB | Console run ingests without error |
|2.3|❌ Create `ai-search.bicep` deploying Azure AI Search (free tier) | `az deployment` succeeds |
|2.4|❌ Build indexer pipeline (Cosmos → Search) & run initial load | `search explorer` returns docs |
|2.5|❌ Add xUnit tests `CatalogSearchTests.cs` (top‑3 recall ≥ 90 %) | `dotnet test` passes |

---

## 3. Planner & RAG Expert Agents (Apr 20–22)
| # | Task | DoD |
|---|------|-----|
|3.1|❌ Implement Planner agent with Azure AI Agent Service SDK (.NET) | Given prompt X, returns tool calls Y |
|3.2|❌ Implement RAG Expert agent with Semantic Kernel & reflection loop | Rouge‑L ≥ 0.7 vs reference |
|3.3|❌ Wire agents into ASP.NET Core API endpoint `/chat` | End‑to‑end response in < 8 s |
|3.4|❌ Add integration test covering Planner→Expert flow | Test passes in CI |

---

## 4. Channel Integrations – Twilio SMS & Voice (Apr 23–24)
| # | Task | DoD |
|---|------|-----|
|4.1|❌ Configure Twilio dev number; set webhook URLs | SMS & Voice webhooks hit Azure logs |
|4.2|❌ Create `SmsWebhook` Azure Function → call `/chat` | Student SMS echoed in response |
|4.3|❌ Create `VoiceWebhook` Azure Function returning <Say> with agent answer | Latency < 10 s |
|4.4|❌ Write Postman collection `sms-voice-e2e.json` | Collection passes both flows |

---

## 5. Action Hooks (Apr 25)
| # | Task | DoD |
|---|------|-----|
|5.1|❌ Graph API helper to schedule Teams meeting | Meeting link returned in JSON |
|5.2|❌ Stub CRM webhook (`/hooks/crm`) & log payload | 200 OK logged with body |
|5.3|❌ Update Planner tool manifest to include `schedule_meeting` & `create_ticket` | Planner selects new tools in test prompt |

---

## 6. UX Polish, Docs & Cost Guardrails (Apr 26–27)
| # | Task | DoD |
|---|------|-----|
|6.1|❌ React chat widget (Vite + Tailwind) hosted on Azure Static Web Apps | Widget connects & passes Lighthouse a11y |
|6.2|❌ Add token budget & Twilio usage limits middleware | Exceeding budget returns friendly error |
|6.3|❌ Write full `README.md` (setup, architecture diagram) | New dev deploys in < 30 min |
|6.4|❌ Add App Insights dashboard (tokens, costs) | Workbook screenshot in `/docs` |

---

## 7. Recording & Submission (Apr 28–30)
| # | Task | DoD |
|---|------|-----|
|7.1|❌ Record demo footage (OBS) following script | Raw footage ≤ 6 min, clear audio |
|7.2|❌ Edit video to ≤ 5 min with captions; export `demo.mp4` | File < 100 MB |
|7.3|❌ Create submission issue using hackathon template; attach links | Checklist passes, repo public |

---

## Backlog / Stretch Goals
- [ ] Cost dashboard workbook
- [ ] FERPA compliance cheatsheet
- [ ] Public dataset fallback (ED.gov College Navigator)
- [ ] A/B evaluation notebook (BLEU/Rouge before/after reflection)

---

> **Workflow tip:** After finishing any task, tick its checkbox, commit, and push. o4‑mini can then list remaining items automatically.

