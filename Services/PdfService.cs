using CrossfitLeaderboard.Models;
using CrossfitLeaderboard.Services.Interfaces;
using CrossfitLeaderboard.Entities;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Borders;
using System.Globalization;

namespace CrossfitLeaderboard.Services
{
    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateLeaderboardPdfAsync(LeaderboardViewModel leaderboard)
        {
            return await Task.Run(() =>
            {
                using MemoryStream ms = new MemoryStream();
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Configurar fonte
                PdfFont font = PdfFontFactory.CreateFont("Helvetica");
                PdfFont boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");

                // Título principal
                Paragraph title = new Paragraph("🏆 CrossFit Leaderboard - Relatório Completo")
                    .SetFont(boldFont)
                    .SetFontSize(24)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(title);

                // Data e hora da geração
                var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
                if (brazilTimeZone == null)
                {
                    // Fallback para sistemas que não têm o timezone configurado
                    brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                }

                var utcNow = DateTime.UtcNow;
                var brazilTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brazilTimeZone);

                Paragraph dateTime = new Paragraph($"Gerado em: {brazilTime.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}")
                    .SetFont(font)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(30);
                document.Add(dateTime);

                // Agrupar times por categoria
                var teamsByCategory = leaderboard.Teams
                    .Where(t => t.Category != null)
                    .GroupBy(t => t.Category)
                    .OrderBy(g => g.Key!.Name)
                    .ToList();

                foreach (var categoryGroup in teamsByCategory)
                {
                    var category = categoryGroup.Key;
                    var teamsInCategory = categoryGroup
                        .OrderBy(t => t.TotalPoints)
                        .ThenByDescending(t => t.FirstPlaceCount)
                        .ThenByDescending(t => t.SecondPlaceCount)
                        .ToList();
                    
                    // Workouts aplicáveis a esta categoria
                    var workoutsForCategory = leaderboard.Workouts
                        .Where(w => w.WorkoutCategories.Any(wc => wc.CategoryId == category!.Id))
                        .ToList();

                    if (teamsInCategory.Any() && workoutsForCategory.Any())
                    {
                        // Título da categoria
                        Paragraph categoryTitle = new Paragraph($"📊 {category!.Name}")
                            .SetFont(boldFont)
                            .SetFontSize(18)
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetMarginTop(20)
                            .SetMarginBottom(10);
                        document.Add(categoryTitle);

                        if (!string.IsNullOrEmpty(category.Description))
                        {
                            Paragraph categoryDesc = new Paragraph(category.Description)
                                .SetFont(font)
                                .SetFontSize(12)
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetMarginBottom(15);
                            document.Add(categoryDesc);
                        }

                        // Criar tabela para a categoria
                        int totalColumns = workoutsForCategory.Count + 2; // +2 para equipe e total
                        Table table = new Table(totalColumns);
                        table.SetWidth(UnitValue.CreatePercentValue(100));
                        table.SetMarginBottom(20);

                        // Configurar larguras das colunas
                        float[] columnWidths = new float[totalColumns];
                        columnWidths[0] = 25f; // Coluna da equipe (25%)
                        
                        // Distribuir o restante entre workouts e total
                        float remainingWidth = 75f;
                        float workoutColumnWidth = remainingWidth / (workoutsForCategory.Count + 1);
                        
                        for (int i = 1; i < totalColumns - 1; i++)
                        {
                            columnWidths[i] = workoutColumnWidth;
                        }
                        columnWidths[totalColumns - 1] = workoutColumnWidth; // Coluna do total
                        
                        table.SetFixedLayout();
                        table.SetWidth(UnitValue.CreatePercentValue(100));

                        // Cabeçalho da tabela
                        Cell headerTeam = new Cell()
                            .Add(new Paragraph("Equipe").SetFont(boldFont).SetFontSize(10))
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            .SetBorder(new SolidBorder(1))
                            .SetPadding(5);
                        table.AddHeaderCell(headerTeam);

                        foreach (var workout in workoutsForCategory)
                        {
                            Cell headerWorkout = new Cell()
                                .Add(new Paragraph(TruncateText(workout.Name, 15)).SetFont(boldFont).SetFontSize(9))
                                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                .SetBorder(new SolidBorder(1))
                                .SetPadding(3);
                            table.AddHeaderCell(headerWorkout);
                        }

                        Cell headerTotal = new Cell()
                            .Add(new Paragraph("Total").SetFont(boldFont).SetFontSize(10))
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            .SetBorder(new SolidBorder(1))
                            .SetPadding(5);
                        table.AddHeaderCell(headerTotal);

                        // Dados das equipes
                        foreach (var team in teamsInCategory)
                        {
                            // Nome da equipe
                            Cell teamCell = new Cell()
                                .Add(new Paragraph($"{team.Id} - {TruncateText(team.Name, 20)}").SetFont(font).SetFontSize(9))
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                .SetBorder(new SolidBorder(0.5f))
                                .SetPadding(3);
                            table.AddCell(teamCell);

                            // Resultados dos workouts
                            foreach (var workout in workoutsForCategory)
                            {
                                var result = leaderboard.ResultsMatrix[team.Id][workout.Id];
                                string resultText = "";

                                if (result.Result.HasValue && result.Result > 0)
                                {
                                    if (workout.Type == WorkoutType.Time)
                                    {
                                        resultText = FormatTimeResult(result.Result.Value);
                                    }
                                    else if (workout.Type == WorkoutType.Repetitions)
                                    {
                                        resultText = FormatInteger(result.Result.Value);
                                    }
                                    else
                                    {
                                        resultText = FormatDecimal(result.Result.Value);
                                    }
                                }

                                // Adicionar posição se houver
                                if (result.Position > 0)
                                {
                                    resultText += $" ({GetPositionText(result.Position)})";
                                }

                                Cell resultCell = new Cell()
                                    .Add(new Paragraph(resultText).SetFont(font).SetFontSize(8))
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                    .SetBorder(new SolidBorder(0.5f))
                                    .SetPadding(2);
                                table.AddCell(resultCell);
                            }

                            // Total de pontos
                            Cell totalCell = new Cell()
                                .Add(new Paragraph(team.TotalPoints.ToString()).SetFont(boldFont).SetFontSize(10))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                .SetBorder(new SolidBorder(0.5f))
                                .SetPadding(3);
                            
                            if (team.TotalPoints > 0)
                            {
                                totalCell.SetBackgroundColor(ColorConstants.GREEN);
                            }
                            table.AddCell(totalCell);
                        }

                        document.Add(table);
                        document.Add(new Paragraph("").SetMarginBottom(20)); // Espaçamento
                    }
                }

                document.Close();
                return ms.ToArray();
            });
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength - 3) + "...";
        }

        private string FormatTimeResult(decimal timeInSeconds)
        {
            int minutes = (int)(timeInSeconds / 60);
            int seconds = (int)(timeInSeconds % 60);
            return $"{minutes}:{seconds:D2}";
        }

        private string FormatInteger(decimal number)
        {
            return ((int)number).ToString();
        }

        private string FormatDecimal(decimal number)
        {
            return number.ToString("N2", new CultureInfo("pt-BR"));
        }

        private string GetPositionText(int position)
        {
            return position switch
            {
                1 => "1º",
                2 => "2º",
                3 => "3º",
                4 => "4º",
                5 => "5º",
                _ => position.ToString() + "º"
            };
        }
    }
} 