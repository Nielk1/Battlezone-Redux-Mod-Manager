using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZRModManager.Audit
{
    /// <summary>
    /// Base class for a single audit. An audit inspects the <see cref="AuditContext"/>
    /// (mod lists, game mod folders, remote audit data) and yields zero or more
    /// actionable <see cref="AuditItem"/> findings. A failure inside one engine is
    /// reported as an item of its own so it can never take down the whole run.
    /// </summary>
    public abstract class AuditEngine
    {
        /// <summary>Short display name of this audit, used for log headers and task names.</summary>
        public abstract string Name { get; }

        /// <summary>What this audit checks, shown in the log header.</summary>
        public virtual string Description { get; }

        /// <summary>
        /// Runs the checks and returns the findings.
        /// </summary>
        protected abstract List<AuditItem> Execute(AuditContext context);

        /// <summary>
        /// Runs <see cref="Execute"/> with failure isolation: an exception thrown by the
        /// engine itself becomes an error item instead of aborting the remaining audits.
        /// </summary>
        public List<AuditItem> Run(AuditContext context)
        {
            try
            {
                return Execute(context);
            }
            catch (Exception ex)
            {
                return new List<AuditItem>
                {
                    new AuditItem
                    {
                        Severity = AuditSeverity.Error,
                        Action = AuditAction.None,
                        Summary = $"audit '{Name}' failed to run: {ex.Message}"
                    }
                };
            }
        }

        /// <summary>The audits that run by default, in order.</summary>
        public static List<AuditEngine> DefaultEngines()
        {
            return new List<AuditEngine>
            {
                new RemoteDataEngine(),
                new IniIntegrityEngine(),
                new DependencyEngine(),
                new DeadJunctionEngine()
            };
        }
    }

    /// <summary>
    /// The result of a full audit run: one section per engine, each holding its findings.
    /// </summary>
    public sealed class AuditReport
    {
        public sealed class Section
        {
            public AuditEngine Engine { get; set; }
            public List<AuditItem> Items { get; } = new List<AuditItem>();
        }

        public List<Section> Sections { get; } = new List<Section>();
        public DateTime Timestamp { get; } = DateTime.Now;

        public void AddSection(AuditEngine engine, IEnumerable<AuditItem> items)
        {
            Section section = new Section
            {
                Engine = engine
            };
            section.Items.AddRange(items);
            Sections.Add(section);
        }

        public int IssueCount => Sections.Sum(dr => dr.Items.Count);
        public int ErrorCount => Sections.Sum(dr => dr.Items.Count(dx => dx.Severity == AuditSeverity.Error));
        public int WarningCount => Sections.Sum(dr => dr.Items.Count(dx => dx.Severity == AuditSeverity.Warning));

        /// <summary>
        /// Renders the whole report for the audit log box / log file. Workshop links
        /// are written on their own "Link:" line so the RichTextBox turns them
        /// into clickable links.
        /// </summary>
        public string ToText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("=============================================\r\n");
            sb.Append("Mod Audit - ").Append(Timestamp.ToString("yyyy-MM-dd HH:mm:ss")).Append("\r\n");
            sb.Append("=============================================\r\n");

            foreach (Section section in Sections)
            {
                sb.Append("\r\n");
                sb.Append("-- ").Append(section.Engine.Name);
                if (!string.IsNullOrWhiteSpace(section.Engine.Description))
                    sb.Append(" - ").Append(section.Engine.Description);
                sb.Append("\r\n");

                if (section.Items.Count == 0)
                {
                    sb.Append("    OK - no issues found\r\n");
                }
                else
                {
                    foreach (AuditItem item in section.Items)
                    {
                        sb.Append("\r\n");
                        foreach (string line in item.ToText().Split(new string[] { "\r\n" }, StringSplitOptions.None))
                            sb.Append("    ").Append(line).Append("\r\n");
                    }
                }
            }

            sb.Append("\r\n=============================================\r\n");
            sb.Append("Summary: ").Append(IssueCount).Append(" issue(s) - ").Append(ErrorCount).Append(" error(s), ");
            sb.Append(WarningCount).Append(" warning(s) - across ").Append(Sections.Count).Append(" audit(s)\r\n");
            sb.Append("=============================================\r\n");

            return sb.ToString();
        }
    }
}