class LeaderboardManager {
    constructor() {
        this.teamsData = []; // Armazenar dados dos times para ordenação
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Event listener para blur (sair do campo)
        $(document).on('blur', '.result-input', (e) => {
            console.log('Blur event triggered');
            this.processInputChange(e.target);
        });

        // Event listener para keydown (pressionar tecla)
        $(document).on('keydown', '.result-input', (e) => {
            // Só processar se for Enter (keyCode 13)
            if (e.keyCode === 13) {
                console.log('Enter key pressed');
                e.preventDefault(); // Prevenir comportamento padrão do Enter
                this.processInputChange(e.target);
                e.target.blur(); // Remover foco do campo
            }
        });

        // Event listener para reset
        $('#resetBtn').on('click', () => {
            if (confirm('Tem certeza que deseja resetar todo o leaderboard? Esta ação não pode ser desfeita.')) {
                this.resetLeaderboard();
            }
        });

        // Máscara para inputs de peso
        $(document).on('input', '.weight-input', (e) => {
            this.applyWeightMask(e.target);
        });
    }

    processInputChange(input) {
        const teamId = parseInt(input.dataset.teamId);
        const workoutId = parseInt(input.dataset.workoutId);
        const workoutType = input.dataset.workoutType;
        
        let result = null;
        
        if (input.value.trim() !== '') {
            switch (workoutType) {
                case 'time':
                    result = this.parseTimeToSeconds(input.value);
                    break;
                case 'repetitions':
                    result = this.parseInteger(input.value);
                    break;
                case 'weight':
                    result = this.parseDecimal(input.value);
                    break;
                default:
                    result = this.parseNumber(input.value);
                    break;
            }
        }
        
        console.log('Processing input change:', { teamId, workoutId, result });
        this.updateResult(teamId, workoutId, result);
    }

    parseInteger(value) {
        const parsed = parseInt(value);
        return isNaN(parsed) ? null : parsed;
    }

    parseDecimal(value) {
        // Converter vírgula para ponto para parsing
        const normalizedValue = value.replace(',', '.');
        const parsed = parseFloat(normalizedValue);
        return isNaN(parsed) ? null : parsed;
    }

    parseNumber(value) {
        const parsed = parseFloat(value);
        return isNaN(parsed) ? null : parsed;
    }

    parseTimeToSeconds(timeString) {
        const match = timeString.match(/^(\d+):(\d{2})$/);
        if (!match) return null;
        
        const minutes = parseInt(match[1]);
        const seconds = parseInt(match[2]);
        
        if (seconds >= 60) return null;
        
        return minutes * 60 + seconds;
    }

    formatSecondsToTime(seconds) {
        if (!seconds || seconds <= 0) return '';
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
    }

    formatInteger(value) {
        return value ? Math.floor(value).toString() : '';
    }

    formatDecimal(value) {
        return value ? value.toFixed(2).replace('.', ',') : '';
    }

    async updateResult(teamId, workoutId, result) {
        try {
            const response = await fetch('/Home/UpdateResult', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    teamId: teamId,
                    workoutId: workoutId,
                    result: result
                })
            });

            if (response.ok) {
                const data = await response.json();
                if (data.success && data.leaderboard) {
                    this.updateLeaderboardDisplay(data.leaderboard);
                } else {
                    console.error('Erro ao atualizar resultado:', data.message);
                }
            } else {
                console.error('Erro ao atualizar resultado');
            }
        } catch (error) {
            console.error('Erro na requisição:', error);
        }
    }

    async resetLeaderboard() {
        try {
            const response = await fetch('/Home/ResetLeaderboard', {
                method: 'POST'
            });

            if (response.ok) {
                const data = await response.json();
                if (data.success) {
                    location.reload();
                } else {
                    console.error('Erro ao resetar leaderboard');
                }
            } else {
                console.error('Erro ao resetar leaderboard');
            }
        } catch (error) {
            console.error('Erro na requisição:', error);
        }
    }

    updateLeaderboardDisplay(leaderboard) {
        // Verificar se leaderboard existe e tem a estrutura esperada
        if (!leaderboard || !leaderboard.results || !leaderboard.teams) {
            console.error('Estrutura de dados inválida:', leaderboard);
            return;
        }

        console.log('Atualizando leaderboard com:', leaderboard.results.length, 'resultados');

        // Armazenar dados dos times para ordenação
        this.teamsData = leaderboard.teams;

        // Limpar todos os badges existentes primeiro
        $('.position-badge').empty();

        // Atualizar resultados
        leaderboard.results.forEach(result => {
            // Usar uma seleção mais específica
            const input = $(`input.result-input[data-team-id="${result.teamId}"][data-workout-id="${result.workoutId}"]`);
            console.log(`Procurando input para team ${result.teamId}, workout ${result.workoutId}:`, input.length > 0 ? 'encontrado' : 'não encontrado');
            
            if (input.length > 0) {
                this.updatePositionBadge(input, result);
            } else {
                console.warn(`Input não encontrado para team ${result.teamId}, workout ${result.workoutId}`);
                // Tentar uma busca alternativa
                const alternativeInput = $(`.result-input[data-team-id="${result.teamId}"][data-workout-id="${result.workoutId}"]`);
                if (alternativeInput.length > 0) {
                    console.log('Input encontrado com busca alternativa');
                    this.updatePositionBadge(alternativeInput, result);
                }
            }
        });

        // Atualizar total de pontos
        this.updateTotalPoints(leaderboard.teams);
        
        // Reordenar times usando critérios de desempate
        this.reorderTeamsByCategory();
    }

    updatePositionBadge(input, result) {
        const cell = input.closest('td');
        let badgeContainer = cell.find('.position-badge');
        
        // Se não encontrar o container, criar um
        if (badgeContainer.length === 0) {
            badgeContainer = $('<div class="position-badge"></div>');
            cell.append(badgeContainer);
        }
        
        console.log(`Atualizando badge para team ${result.teamId}, workout ${result.workoutId}, posição ${result.position}`);
        
        // Remover badge existente
        badgeContainer.empty();
        
        // Adicionar novo badge se houver posição
        if (result.position > 0) {
            const badgeClass = this.getPositionBadgeClass(result.position);
            const badgeText = this.getPositionText(result.position);
            const badge = $(`<span class="badge ${badgeClass}">${badgeText}</span>`);
            badgeContainer.append(badge);
            console.log(`Badge adicionado: ${badgeText} com classe ${badgeClass}`);
        } else {
            console.log('Nenhum badge adicionado - posição 0 ou inválida');
        }
    }

    updateTotalPoints(teams) {
        teams.forEach(team => {
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
        // Reordenar times dentro de cada categoria usando critérios de desempate
        $('.leaderboard-table').each((index, table) => {
            const tbody = $(table).find('tbody');
            const rows = tbody.find('tr').get();
            
            rows.sort((a, b) => {
                // Encontrar os dados dos times correspondentes
                const aTeamId = parseInt($(a).find('.result-input').first().data('team-id'));
                const bTeamId = parseInt($(b).find('.result-input').first().data('team-id'));
                
                const aTeam = this.teamsData.find(t => t.id === aTeamId);
                const bTeam = this.teamsData.find(t => t.id === bTeamId);
                
                if (!aTeam || !bTeam) {
                    console.warn('Time não encontrado nos dados:', { aTeamId, bTeamId });
                    return 0;
                }
                
                // Primeiro critério: total de pontos (menor é melhor)
                if (aTeam.totalPoints !== bTeam.totalPoints) {
                    return aTeam.totalPoints - bTeam.totalPoints;
                }
                
                // Segundo critério: número de 1º lugares (maior é melhor)
                if (aTeam.firstPlaceCount !== bTeam.firstPlaceCount) {
                    return bTeam.firstPlaceCount - aTeam.firstPlaceCount;
                }
                
                // Terceiro critério: número de 2º lugares (maior é melhor)
                return bTeam.secondPlaceCount - aTeam.secondPlaceCount;
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