class LeaderboardManager {
    constructor() {
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Atualizar resultado quando o input mudar
        $(document).on('change', '.result-input', (e) => {
            const input = $(e.target);
            const teamId = input.data('team-id');
            const workoutId = input.data('workout-id');
            const workoutType = input.data('workout-type');
            
            let result = null;
            const inputValue = input.val().trim();
            
            if (inputValue === '') {
                // Campo vazio = null (workout não feito)
                result = null;
            } else if (workoutType === 'time') {
                result = this.parseTimeToSeconds(inputValue);
            } else if (workoutType === 'repetitions') {
                result = this.parseInteger(inputValue);
            } else if (workoutType === 'weight') {
                result = this.parseDecimal(inputValue);
            } else {
                result = this.parseNumber(inputValue);
            }

            this.updateResult(teamId, workoutId, result);
        });

        // Máscara para inputs de peso
        $(document).on('input', '.weight-input', (e) => {
            this.applyWeightMask(e.target);
        });

        // Reset do leaderboard
        $('#resetBtn').on('click', () => {
            if (confirm('Tem certeza que deseja resetar o leaderboard?')) {
                this.resetLeaderboard();
            }
        });
    }

    parseInteger(value) {
        if (!value || value.trim() === '') return 0;
        
        // Remover espaços e converter vírgula para ponto
        const cleanValue = value.toString().trim().replace(',', '.');
        const parsed = parseInt(cleanValue);
        
        return isNaN(parsed) ? 0 : parsed;
    }

    parseDecimal(value) {
        if (!value || value.trim() === '') return 0;
        
        // Remover espaços e converter vírgula para ponto
        const cleanValue = value.toString().trim().replace(',', '.');
        const parsed = parseFloat(cleanValue);
        
        return isNaN(parsed) ? 0 : parsed;
    }

    parseNumber(value) {
        if (!value || value.trim() === '') return 0;
        
        // Remover espaços e converter vírgula para ponto
        const cleanValue = value.toString().trim().replace(',', '.');
        const parsed = parseFloat(cleanValue);
        
        return isNaN(parsed) ? 0 : parsed;
    }

    parseTimeToSeconds(timeString) {
        if (!timeString || timeString.trim() === '') return 0;
        
        // Formato esperado: "min:sec" (ex: "10:30")
        const parts = timeString.split(':');
        if (parts.length !== 2) return 0;
        
        const minutes = parseInt(parts[0]) || 0;
        const seconds = parseInt(parts[1]) || 0;
        
        return minutes * 60 + seconds;
    }

    formatSecondsToTime(seconds) {
        if (seconds <= 0) return '';
        
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = Math.floor(seconds % 60);
        return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
    }

    formatInteger(value) {
        if (value <= 0) return '';
        return Math.floor(value).toString();
    }

    formatDecimal(value) {
        if (value <= 0) return '';
        // Formatar com vírgula para exibição (formato brasileiro)
        return value.toFixed(2).replace('.', ',');
    }

    updateResult(teamId, workoutId, result) {
        $.ajax({
            url: '/Home/UpdateResult',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                teamId: teamId,
                workoutId: workoutId,
                result: result // Pode ser null, 0, ou um valor positivo
            }),
            success: (response) => {
                if (response.success) {
                    this.updateLeaderboardDisplay(response.leaderboard);
                } else {
                    alert('Erro ao atualizar resultado: ' + response.message);
                }
            },
            error: (xhr, status, error) => {
                console.error('Erro AJAX:', xhr.responseText);
                alert('Erro ao comunicar com o servidor. Verifique o console para mais detalhes.');
            }
        });
    }

    resetLeaderboard() {
        $.ajax({
            url: '/Home/ResetLeaderboard',
            type: 'POST',
            success: (response) => {
                if (response.success) {
                    this.updateLeaderboardDisplay(response.leaderboard);
                    $('.result-input').val('');
                    
                    // Clear all total points cells
                    $('td:last-child').each(function() {
                        $(this).text('0');
                        $(this).removeClass('bg-success text-white');
                    });
                    
                    // Remove all position badges
                    $('.position-badge').remove();
                } else {
                    alert('Erro ao resetar leaderboard');
                }
            },
            error: (xhr, status, error) => {
                console.error('Erro AJAX:', xhr.responseText);
                alert('Erro ao comunicar com o servidor. Verifique o console para mais detalhes.');
            }
        });
    }

    updateLeaderboardDisplay(leaderboard) {
        // Atualizar valores dos inputs
        leaderboard.teams.forEach((team) => {
            leaderboard.workouts.forEach((workout) => {
                const result = leaderboard.results.find(r => 
                    r.teamId === team.id && r.workoutId === workout.id);
                
                const input = $(`.result-input[data-team-id="${team.id}"][data-workout-id="${workout.id}"]`);
                if (input.length > 0) {
                    // Verificar se o resultado existe antes de acessar suas propriedades
                    if (result) {
                        const workoutType = input.data('workout-type');
                        
                        if (result.result === null) {
                            // Resultado null = workout não feito, deixar campo vazio
                            input.val('');
                        } else if (workoutType === 'time') {
                            // Para inputs de tempo, converter segundos para formato min:sec
                            input.val(result.result > 0 ? this.formatSecondsToTime(result.result) : '');
                        } else if (workoutType === 'repetitions') {
                            // Para repetições, exibir como inteiro
                            input.val(result.result > 0 ? this.formatInteger(result.result) : '');
                        } else if (workoutType === 'weight') {
                            // Para peso, exibir como decimal com vírgula
                            input.val(result.result > 0 ? this.formatDecimal(result.result) : '');
                        } else {
                            // Fallback para outros tipos
                            const numericValue = this.parseNumber(result.result);
                            if (numericValue > 0) {
                                input.val(this.formatDecimal(numericValue));
                            } else {
                                input.val('');
                            }
                        }
                        // Atualizar badge de posição
                        this.updatePositionBadge(input, result);
                    } else {
                        // Se não há resultado, limpar o input e remover badge
                        input.val('');
                        this.updatePositionBadge(input, { position: 0, result: null });
                    }
                }
            });
        });

        // Atualizar pontos totais e reordenar por categoria
        this.updateTotalPoints(leaderboard.teams);
        this.reorderTeamsByCategory();
    }

    updatePositionBadge(input, result) {
        const cell = input.closest('td');
        const positionBadge = cell.find('.position-badge');
        
        if (result && result.position > 0) {
            if (positionBadge.length === 0) {
                cell.append(`<div class="position-badge"><span class="badge ${this.getPositionBadgeClass(result.position)}">${this.getPositionText(result.position)}</span></div>`);
            } else {
                positionBadge.find('.badge')
                    .removeClass()
                    .addClass(`badge ${this.getPositionBadgeClass(result.position)}`)
                    .text(this.getPositionText(result.position));
            }
        } else {
            positionBadge.remove();
        }
    }

    updateTotalPoints(teams) {
        teams.forEach((team) => {
            const totalCell = $(`tr:has(.result-input[data-team-id="${team.id}"]) td:last`);
            if (totalCell.length > 0) {
                totalCell.text(team.totalPoints);
                totalCell.removeClass('bg-success text-white');
                if (team.totalPoints > 0) {
                    totalCell.addClass('bg-success text-white');
                }
            }
        });
    }

    reorderTeamsByCategory() {
        // Reordenar times dentro de cada categoria
        $('.leaderboard-table').each((index, table) => {
            const tbody = $(table).find('tbody');
            const rows = tbody.find('tr').get();
            rows.sort((a, b) => {
                const aPoints = parseInt($(a).find('td:last').text()) || 0;
                const bPoints = parseInt($(b).find('td:last').text()) || 0;
                return aPoints - bPoints;
            });
            tbody.empty().append(rows);
        });
    }

    getPositionBadgeClass(position) {
        switch(position) {
            case 1: return 'bg-warning text-dark';
            case 2: return 'bg-secondary';
            case 3: return 'bg-warning text-dark';
            default: return 'bg-secondary';
        }
    }

    getPositionText(position) {
        switch(position) {
            case 1: return '🥇 1º';
            case 2: return '🥈 2º';
            case 3: return '🥉 3º';
            case 4: return '4º';
            case 5: return '5º';
            default: return '';
        }
    }

    applyWeightMask(input) {
        let value = input.value;
        
        // Remover tudo exceto números e vírgula
        value = value.replace(/[^\d,]/g, '');
        
        // Garantir que só há uma vírgula
        const commaCount = (value.match(/,/g) || []).length;
        if (commaCount > 1) {
            value = value.replace(/,/g, (match, index) => {
                return index === value.indexOf(',') ? ',' : '';
            });
        }
        
        // Limitar a duas casas decimais após a vírgula
        if (value.includes(',')) {
            const parts = value.split(',');
            if (parts[1] && parts[1].length > 2) {
                parts[1] = parts[1].substring(0, 2);
                value = parts.join(',');
            }
        }
        
        // Se o valor começa com vírgula, adicionar 0 na frente
        if (value.startsWith(',')) {
            value = '0' + value;
        }
        
        input.value = value;
    }
}

// Inicializar quando o documento estiver pronto
$(document).ready(() => {
    new LeaderboardManager();
}); 